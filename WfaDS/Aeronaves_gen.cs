using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data;

namespace WfaDS
{
    public partial class Aeronaves_gen : Form
    {
        private enum FormState
        {
            Inicial,
            Adicionando,
            Editando,
            Visualizando
        }

        private FormState estadoAtual = FormState.Inicial;
        private string caminhoImagemSelecionada = "";
        private bool dadosCarregados = false;
        private int idOriginal = -1;
        private DataTable dtAeronaves;

        private LP2DataSet dataset;
        private LP2DataSetTableAdapters.AeronaveTableAdapter aeronaveTableAdapter;

        public Aeronaves_gen()
        {
            InitializeComponent();

            dataset = new LP2DataSet();
            aeronaveTableAdapter = new LP2DataSetTableAdapters.AeronaveTableAdapter();

            dtAeronaves = new DataTable();
            dtAeronaves.Columns.Add("ID", typeof(int));
            dtAeronaves.Columns.Add("Prefixo", typeof(string));
            dtAeronaves.Columns.Add("Modelo", typeof(string));
            dtAeronaves.Columns.Add("Tripulação", typeof(string));
            dtAeronaves.Columns.Add("Data Fabricação", typeof(DateTime));
            dtAeronaves.Columns.Add("Idade", typeof(int));
            dgvAeronaves.CellClick += dgvAeronaves_CellClick;

            ConfigurarDataGridView();
            ConfigurarEstadoInicial();
        }

        private void ConfigurarDataGridView()
        {
            dgvAeronaves.AutoGenerateColumns = false;
            dgvAeronaves.DataSource = dtAeronaves;

            dgvAeronaves.Columns.Clear();

            DataGridViewTextBoxColumn colId = new DataGridViewTextBoxColumn();
            colId.DataPropertyName = "ID";
            colId.HeaderText = "ID";
            colId.Width = 50;
            dgvAeronaves.Columns.Add(colId);

            DataGridViewTextBoxColumn colPrefixo = new DataGridViewTextBoxColumn();
            colPrefixo.DataPropertyName = "Prefixo";
            colPrefixo.HeaderText = "Prefixo";
            colPrefixo.Width = 80;
            dgvAeronaves.Columns.Add(colPrefixo);

            DataGridViewTextBoxColumn colModelo = new DataGridViewTextBoxColumn();
            colModelo.DataPropertyName = "Modelo";
            colModelo.HeaderText = "Modelo";
            colModelo.Width = 120;
            dgvAeronaves.Columns.Add(colModelo);

            DataGridViewTextBoxColumn colTripulacao = new DataGridViewTextBoxColumn();
            colTripulacao.DataPropertyName = "Tripulação";
            colTripulacao.HeaderText = "Tripulação";
            colTripulacao.Width = 80;
            dgvAeronaves.Columns.Add(colTripulacao);

            DataGridViewTextBoxColumn colDataFab = new DataGridViewTextBoxColumn();
            colDataFab.DataPropertyName = "Data Fabricação";
            colDataFab.HeaderText = "Data Fabricação";
            colDataFab.DefaultCellStyle.Format = "dd/MM/yyyy";
            colDataFab.Width = 100;
            dgvAeronaves.Columns.Add(colDataFab);

            DataGridViewTextBoxColumn colIdade = new DataGridViewTextBoxColumn();
            colIdade.DataPropertyName = "Idade";
            colIdade.HeaderText = "Idade (anos)";
            colIdade.Width = 60;
            dgvAeronaves.Columns.Add(colIdade);

            dgvAeronaves.ReadOnly = true;
            dgvAeronaves.AllowUserToAddRows = false;
            dgvAeronaves.AllowUserToDeleteRows = false;
            dgvAeronaves.EditMode = DataGridViewEditMode.EditProgrammatically;
            dgvAeronaves.MultiSelect = false;
            dgvAeronaves.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgvAeronaves.EnableHeadersVisualStyles = false;
            dgvAeronaves.ColumnHeadersDefaultCellStyle.BackColor = Color.Navy;
            dgvAeronaves.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvAeronaves.ColumnHeadersDefaultCellStyle.Font = new Font(dgvAeronaves.Font, FontStyle.Bold);
            dgvAeronaves.RowHeadersVisible = false;
            dgvAeronaves.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        #region Configuração de Estados

        private void ConfigurarEstadoInicial()
        {
            btnAdicionar.Enabled = true;
            btnVisualizar.Enabled = true;
            btnAtualizar.Enabled = true;
            btnDeletar.Enabled = true;
            btnSalvar.Enabled = false;
            btnCancelar.Enabled = false;
            btnMenu.Enabled = true;
            btnFoto.Enabled = false;

            // SEMPRE desativado - ID é autoincrement
            txtID.Enabled = false;
            txtID.BackColor = SystemColors.Control;
            txtPrefixo.Enabled = false;
            txtModelo.Enabled = false;
            txtTripulacao.Enabled = false;
            dtpData.Enabled = false;
            pbFoto.Enabled = false;

            LimparCampos();

            estadoAtual = FormState.Inicial;
        }

        private void ConfigurarEstadoAdicionar()
        {
            btnAdicionar.Enabled = false;
            btnVisualizar.Enabled = false;
            btnAtualizar.Enabled = false;
            btnDeletar.Enabled = false;
            btnSalvar.Enabled = true;
            btnCancelar.Enabled = true;
            btnMenu.Enabled = false;
            btnFoto.Enabled = true;

            // ID SEMPRE desativado - será gerado automaticamente
            txtID.Enabled = false;
            txtID.BackColor = SystemColors.Control;
            txtID.Text = "(Será gerado automaticamente)";
            txtID.ForeColor = Color.Gray;

            txtPrefixo.Enabled = true;
            txtModelo.Enabled = true;
            txtTripulacao.Enabled = true;
            dtpData.Enabled = true;
            pbFoto.Enabled = true;

            LimparCampos();
            txtPrefixo.Focus(); // Foca no prefixo em vez do ID

            estadoAtual = FormState.Adicionando;
        }

        private void ConfigurarEstadoEditar()
        {
            btnAdicionar.Enabled = false;
            btnVisualizar.Enabled = false;
            btnAtualizar.Enabled = false;
            btnDeletar.Enabled = false;
            btnSalvar.Enabled = true;
            btnSalvar.Text = "Confirmar";
            btnCancelar.Enabled = true;
            btnMenu.Enabled = false;
            btnFoto.Enabled = true;

            // ID SEMPRE desativado em edição também
            txtID.Enabled = false;
            txtID.BackColor = SystemColors.Control;
            txtPrefixo.Enabled = true;
            txtModelo.Enabled = true;
            txtTripulacao.Enabled = true;
            dtpData.Enabled = true;
            pbFoto.Enabled = true;

            estadoAtual = FormState.Editando;
        }

        private void ConfigurarEstadoVisualizar()
        {
            btnAdicionar.Enabled = false;
            btnVisualizar.Enabled = false;
            btnAtualizar.Enabled = false;
            btnDeletar.Enabled = false;
            btnSalvar.Enabled = false;
            btnCancelar.Enabled = true;
            btnMenu.Enabled = false;
            btnFoto.Enabled = false;

            // Todos os campos desativados na visualização
            txtID.Enabled = false;
            txtID.BackColor = SystemColors.Control;
            txtPrefixo.Enabled = false;
            txtModelo.Enabled = false;
            txtTripulacao.Enabled = false;
            dtpData.Enabled = false;
            pbFoto.Enabled = false;

            estadoAtual = FormState.Visualizando;
        }

        private void LimparCampos()
        {
            txtID.Clear();
            txtPrefixo.Clear();
            txtModelo.Clear();
            txtTripulacao.Clear();
            dtpData.Value = DateTime.Now;
            pbFoto.Image = null;
            caminhoImagemSelecionada = "";
            idOriginal = -1;
        }

        #endregion

        #region Eventos dos Botões

        private void btnAdicionar_Click(object sender, EventArgs e)
        {
            try
            {
                ConfigurarEstadoAdicionar();
                MessageBox.Show("Preencha os dados da nova aeronave. O ID será gerado automaticamente.", "Adicionar Aeronave", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao iniciar adição: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnVisualizar_Click(object sender, EventArgs e)
        {
            try
            {
                dgvAeronaves.Visible = true;
                CarregarTodasAeronaves();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao visualizar aeronaves: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAtualizar_Click(object sender, EventArgs e)
        {
            try
            {
                string input = ShowInputDialog("Digite o ID da aeronave que deseja editar:", "Editar Aeronave");

                if (!string.IsNullOrEmpty(input) && int.TryParse(input, out int id))
                {
                    if (CarregarAeronave(id))
                    {
                        idOriginal = id;
                        ConfigurarEstadoEditar();
                        MessageBox.Show("Dados carregados. Faça as alterações e clique em Confirmar.", "Editar Aeronave", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Aeronave não encontrada!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar aeronave para edição: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDeletar_Click(object sender, EventArgs e)
        {
            try
            {
                string input = ShowInputDialog("Digite o ID da aeronave que deseja deletar:", "Deletar Aeronave");

                if (!string.IsNullOrEmpty(input) && int.TryParse(input, out int id))
                {
                    if (DeletarAeronave(id))
                    {
                        MessageBox.Show("Aeronave deletada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ConfigurarEstadoInicial();
                        if (dgvAeronaves.Visible)
                        {
                            CarregarTodasAeronaves();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao deletar aeronave: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidarCampos())
                    return;

                if (estadoAtual == FormState.Adicionando)
                {
                    if (AdicionarAeronave())
                    {
                        MessageBox.Show("Aeronave adicionada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ConfigurarEstadoInicial();
                        if (dgvAeronaves.Visible)
                        {
                            CarregarTodasAeronaves();
                        }
                    }
                }
                else if (estadoAtual == FormState.Editando)
                {
                    if (AtualizarAeronave())
                    {
                        MessageBox.Show("Aeronave atualizada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        btnSalvar.Text = "Salvar";
                        ConfigurarEstadoInicial();
                        if (dgvAeronaves.Visible)
                        {
                            CarregarTodasAeronaves();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Deseja cancelar a operação? Todos os dados não salvos serão perdidos.",
                    "Cancelar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    btnSalvar.Text = "Salvar";
                    ConfigurarEstadoInicial();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao cancelar: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao fechar tela: {ex.Message}");
                MessageBox.Show("Ocorreu um erro ao tentar fechar a tela. Tente novamente.");
            }
        }

        private void btnFoto_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Arquivos de Imagem|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                    openFileDialog.Title = "Selecionar Foto da Aeronave";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        caminhoImagemSelecionada = openFileDialog.FileName;
                        pbFoto.Image = Image.FromFile(caminhoImagemSelecionada);
                        pbFoto.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar imagem: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtID_TextChanged(object sender, EventArgs e)
        {
            // Método mantido apenas para compatibilidade com o Designer
            // Não faz mais validação pois o ID é autoincrement
        }

        private bool ValidarCampos()
        {
            // REMOVIDA validação do ID - não é mais necessário

            if (string.IsNullOrWhiteSpace(txtPrefixo.Text))
            {
                MessageBox.Show("O campo Prefixo é obrigatório!", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrefixo.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtModelo.Text))
            {
                MessageBox.Show("O campo Modelo é obrigatório!", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtModelo.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtTripulacao.Text))
            {
                MessageBox.Show("O campo Tripulação é obrigatório!", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTripulacao.Focus();
                return false;
            }

            return true;
        }

        // REMOVIDO método IdJaExiste - não é mais necessário

        private bool AdicionarAeronave()
        {
            try
            {
                // O ID será gerado automaticamente pelo banco de dados
                var newRow = dataset.Aeronave.NewAeronaveRow();
                // NÃO definimos o Id - será autoincrement
                newRow.Prefixo = txtPrefixo.Text.Trim();
                newRow.Modelo = txtModelo.Text.Trim();
                newRow.Tripulacao = txtTripulacao.Text.Trim();
                newRow.DataFab = dtpData.Value;

                if (!string.IsNullOrEmpty(caminhoImagemSelecionada))
                {
                    newRow.Foto = caminhoImagemSelecionada;
                }
                else
                {
                    newRow.Foto = string.Empty;
                }

                dataset.Aeronave.AddAeronaveRow(newRow);
                aeronaveTableAdapter.Update(dataset.Aeronave);

                // Mostra o ID que foi gerado automaticamente
                int novoId = newRow.Id;
                MessageBox.Show($"Aeronave adicionada com sucesso!\nID gerado automaticamente: {novoId}", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao adicionar aeronave: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool CarregarAeronave(int id)
        {
            try
            {
                dataset.Aeronave.Clear();
                aeronaveTableAdapter.Fill(dataset.Aeronave);

                var aeronave = dataset.Aeronave.AsEnumerable().FirstOrDefault(a => a.Field<int>("Id") == id);

                if (aeronave != null)
                {
                    txtID.Text = aeronave.Field<int>("Id").ToString();
                    txtPrefixo.Text = aeronave.Field<string>("Prefixo") ?? "";
                    txtModelo.Text = aeronave.Field<string>("Modelo") ?? "";
                    txtTripulacao.Text = aeronave.Field<string>("Tripulacao") ?? "";
                    dtpData.Value = aeronave.Field<DateTime>("DataFab");

                    string caminhoFoto = aeronave.Field<string>("Foto");
                    if (!string.IsNullOrEmpty(caminhoFoto) && File.Exists(caminhoFoto))
                    {
                        try
                        {
                            pbFoto.Image = Image.FromFile(caminhoFoto);
                            pbFoto.SizeMode = PictureBoxSizeMode.StretchImage;
                            caminhoImagemSelecionada = caminhoFoto;
                        }
                        catch
                        {
                            pbFoto.Image = null;
                            caminhoImagemSelecionada = "";
                        }
                    }
                    else
                    {
                        pbFoto.Image = null;
                        caminhoImagemSelecionada = "";
                    }

                    dadosCarregados = true;
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar aeronave: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void CarregarTodasAeronaves()
        {
            try
            {
                dataset.Aeronave.Clear();
                aeronaveTableAdapter.Fill(dataset.Aeronave);

                dtAeronaves.Rows.Clear();

                if (dataset.Aeronave.Rows.Count == 0)
                {
                    MessageBox.Show("Nenhuma aeronave cadastrada no sistema.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                foreach (DataRow row in dataset.Aeronave.Rows)
                {
                    int id = Convert.ToInt32(row["Id"]);
                    string prefixo = row["Prefixo"]?.ToString() ?? "N/A";
                    string modelo = row["Modelo"]?.ToString() ?? "N/A";
                    string tripulacao = row["Tripulacao"]?.ToString() ?? "N/A";
                    DateTime dataFab = Convert.ToDateTime(row["DataFab"]);

                    int idade = DateTime.Today.Year - dataFab.Year;
                    if (dataFab.Date > DateTime.Today.AddYears(-idade)) idade--;

                    dtAeronaves.Rows.Add(id, prefixo, modelo, tripulacao, dataFab, idade);
                }

                dgvAeronaves.ReadOnly = true;
                dgvAeronaves.AllowUserToAddRows = false;
                dgvAeronaves.AllowUserToDeleteRows = false;

                dgvAeronaves.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar a lista de aeronaves: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool AtualizarAeronave()
        {
            try
            {
                if (idOriginal == -1)
                {
                    MessageBox.Show("Erro: ID original não foi definido!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                string prefixo = txtPrefixo.Text.Trim();
                string modelo = txtModelo.Text.Trim();
                string tripulacao = txtTripulacao.Text.Trim();
                DateTime dataFab = dtpData.Value;

                string caminhoFoto = caminhoImagemSelecionada;

                if (string.IsNullOrEmpty(caminhoFoto))
                {
                    dataset.Aeronave.Clear();
                    aeronaveTableAdapter.Fill(dataset.Aeronave);
                    var aeronaveAtual = dataset.Aeronave.AsEnumerable().FirstOrDefault(a => a.Field<int>("Id") == idOriginal);
                    if (aeronaveAtual != null)
                    {
                        caminhoFoto = aeronaveAtual.Field<string>("Foto") ?? string.Empty;
                    }
                }

                var rowToUpdate = dataset.Aeronave.AsEnumerable().FirstOrDefault(a => a.Field<int>("Id") == idOriginal);
                if (rowToUpdate != null)
                {
                    rowToUpdate["Prefixo"] = prefixo;
                    rowToUpdate["Modelo"] = modelo;
                    rowToUpdate["Tripulacao"] = tripulacao;
                    rowToUpdate["DataFab"] = dataFab;
                    rowToUpdate["Foto"] = caminhoFoto;

                    aeronaveTableAdapter.Update(dataset.Aeronave);

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao atualizar aeronave: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool DeletarAeronave(int id)
        {
            try
            {
                dataset.Aeronave.Clear();
                aeronaveTableAdapter.Fill(dataset.Aeronave);
                var aeronave = dataset.Aeronave.AsEnumerable().FirstOrDefault(a => a.Field<int>("Id") == id);

                if (aeronave == null)
                {
                    MessageBox.Show("Aeronave não encontrada!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                string prefixoAeronave = aeronave.Field<string>("Prefixo") ?? "[Prefixo não disponível]";

                if (MessageBox.Show($"Tem certeza que deseja deletar a aeronave:\n\nID: {id}\nPrefixo: {prefixoAeronave}",
                    "Confirmar Exclusão", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var rowToDelete = dataset.Aeronave.AsEnumerable().FirstOrDefault(a => a.Field<int>("Id") == id);
                    if (rowToDelete != null)
                    {
                        rowToDelete.Delete();

                        aeronaveTableAdapter.Update(dataset.Aeronave);

                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao deletar aeronave: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void dtpData_ValueChanged(object sender, EventArgs e)
        {
            if (dtpData.Value > DateTime.Now)
            {
                MessageBox.Show("A data de fabricação não pode ser futura!", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpData.Value = DateTime.Now;
            }
        }

        private void pbFoto_Click(object sender, EventArgs e)
        {
            if (estadoAtual == FormState.Adicionando || estadoAtual == FormState.Editando)
            {
                btnFoto_Click(sender, e);
            }
        }

        private void txtPrefixo_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPrefixo.Text))
            {
                txtPrefixo.ForeColor = Color.Red;
            }
            else
            {
                txtPrefixo.ForeColor = SystemColors.WindowText;
            }
        }

        private void txtModelo_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtModelo.Text))
            {
                txtModelo.ForeColor = Color.Red;
            }
            else
            {
                txtModelo.ForeColor = SystemColors.WindowText;
            }
        }

        private void txtTripulação_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTripulacao.Text))
            {
                txtTripulacao.ForeColor = Color.Red;
            }
            else
            {
                txtTripulacao.ForeColor = SystemColors.WindowText;
            }
        }

        private void CarregarImagemAeronave(int idAeronave)
        {
            try
            {
                dataset.Aeronave.Clear();
                aeronaveTableAdapter.Fill(dataset.Aeronave);

                var aeronave = dataset.Aeronave.AsEnumerable()
                                   .FirstOrDefault(a => a.Field<int>("Id") == idAeronave);

                if (aeronave != null)
                {
                    string caminhoFoto = aeronave.Field<string>("Foto");
                    if (!string.IsNullOrEmpty(caminhoFoto) && File.Exists(caminhoFoto))
                    {
                        pbFoto.Image = Image.FromFile(caminhoFoto);
                        pbFoto.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                    else
                    {
                        pbFoto.Image = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar imagem da aeronave: {ex.Message}", "Erro",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                pbFoto.Image = null;
            }
        }

        private void dgvAeronaves_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    string nomeColuna = dgvAeronaves.Columns[e.ColumnIndex].DataPropertyName;

                    if (nomeColuna.Equals("ID", StringComparison.OrdinalIgnoreCase))
                    {
                        int idAeronave = Convert.ToInt32(dgvAeronaves.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                        CarregarImagemAeronave(idAeronave);

                        CarregarAeronave(idAeronave);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao selecionar aeronave: {ex.Message}", "Erro",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Carregamento do Form

        private void Aeronaves_gen_Load(object sender, EventArgs e)
        {
            try
            {
                aeronaveTableAdapter.Fill(dataset.Aeronave);
                ConfigurarEstadoInicial();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar dados: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string ShowInputDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 400,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen,
                MaximizeBox = false,
                MinimizeBox = false
            };

            Label textLabel = new Label() { Left = 20, Top = 20, Width = 350, Text = text };
            TextBox inputBox = new TextBox() { Left = 20, Top = 50, Width = 250 };
            Button confirmation = new Button() { Text = "OK", Left = 280, Width = 70, Top = 48, DialogResult = DialogResult.OK };
            Button cancel = new Button() { Text = "Cancel", Left = 280, Width = 70, Top = 76, DialogResult = DialogResult.Cancel };

            confirmation.Click += (sender, e) => { prompt.Close(); };
            cancel.Click += (sender, e) => { prompt.Close(); };

            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(inputBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(cancel);
            prompt.AcceptButton = confirmation;
            prompt.CancelButton = cancel;

            return prompt.ShowDialog() == DialogResult.OK ? inputBox.Text : "";
        }

        #endregion
    }
}