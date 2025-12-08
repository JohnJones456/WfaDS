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
using WfaDS.LP2DataSetTableAdapters;

namespace WfaDS
{
    public partial class Pilotos_gen : Form
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
        private DataTable dtPilotos;

        private LP2DataSet dataset;
        private LP2DataSetTableAdapters.PilotoTableAdapter pilotoTableAdapter;

        public Pilotos_gen()
        {
            InitializeComponent();

            dataset = new LP2DataSet();
            pilotoTableAdapter = new LP2DataSetTableAdapters.PilotoTableAdapter();

            dtPilotos = new DataTable();
            dtPilotos.Columns.Add("ID", typeof(int));
            dtPilotos.Columns.Add("Nome", typeof(string));
            dtPilotos.Columns.Add("Breve", typeof(string));
            dtPilotos.Columns.Add("Nacionalidade", typeof(string));
            dtPilotos.Columns.Add("Data Nascimento", typeof(DateTime));
            dtPilotos.Columns.Add("Idade", typeof(int));
            dgvPilotos.CellClick += dgvPilotos_CellClick;

            ConfigurarDataGridView();
            ConfigurarEstadoInicial();
        }

        private void ConfigurarDataGridView()
        {
            dgvPilotos.AutoGenerateColumns = false;
            dgvPilotos.DataSource = dtPilotos;

            dgvPilotos.Columns.Clear();

            DataGridViewTextBoxColumn colId = new DataGridViewTextBoxColumn();
            colId.DataPropertyName = "ID";
            colId.HeaderText = "ID";
            colId.Width = 50;
            dgvPilotos.Columns.Add(colId);

            DataGridViewTextBoxColumn colNome = new DataGridViewTextBoxColumn();
            colNome.DataPropertyName = "Nome";
            colNome.HeaderText = "Nome";
            colNome.Width = 150;
            dgvPilotos.Columns.Add(colNome);

            DataGridViewTextBoxColumn colBreve = new DataGridViewTextBoxColumn();
            colBreve.DataPropertyName = "Breve";
            colBreve.HeaderText = "Breve";
            colBreve.Width = 80;
            dgvPilotos.Columns.Add(colBreve);

            DataGridViewTextBoxColumn colNacionalidade = new DataGridViewTextBoxColumn();
            colNacionalidade.DataPropertyName = "Nacionalidade";
            colNacionalidade.HeaderText = "Nacionalidade";
            colNacionalidade.Width = 100;
            dgvPilotos.Columns.Add(colNacionalidade);

            DataGridViewTextBoxColumn colDataNasc = new DataGridViewTextBoxColumn();
            colDataNasc.DataPropertyName = "Data Nascimento";
            colDataNasc.HeaderText = "Data Nascimento";
            colDataNasc.DefaultCellStyle.Format = "dd/MM/yyyy";
            colDataNasc.Width = 100;
            dgvPilotos.Columns.Add(colDataNasc);

            DataGridViewTextBoxColumn colIdade = new DataGridViewTextBoxColumn();
            colIdade.DataPropertyName = "Idade";
            colIdade.HeaderText = "Idade (anos)";
            colIdade.Width = 60;
            dgvPilotos.Columns.Add(colIdade);

            dgvPilotos.ReadOnly = true;
            dgvPilotos.AllowUserToAddRows = false;
            dgvPilotos.AllowUserToDeleteRows = false;
            dgvPilotos.EditMode = DataGridViewEditMode.EditProgrammatically;
            dgvPilotos.MultiSelect = false;
            dgvPilotos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgvPilotos.EnableHeadersVisualStyles = false;
            dgvPilotos.ColumnHeadersDefaultCellStyle.BackColor = Color.Navy;
            dgvPilotos.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvPilotos.ColumnHeadersDefaultCellStyle.Font = new Font(dgvPilotos.Font, FontStyle.Bold);
            dgvPilotos.RowHeadersVisible = false;
            dgvPilotos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
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
            txtID.Enabled = false;
            txtID.BackColor = SystemColors.Control;
            txtNome.Enabled = false;
            txtBreve.Enabled = false;
            txtNacionalidade.Enabled = false;
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
            txtID.Enabled = false;
            txtID.BackColor = SystemColors.Control;
            txtID.Text = "(Será gerado automaticamente)";
            txtID.ForeColor = Color.Gray;

            txtNome.Enabled = true;
            txtBreve.Enabled = true;
            txtNacionalidade.Enabled = true;
            dtpData.Enabled = true;
            pbFoto.Enabled = true;

            LimparCampos();
            txtNome.Focus();

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
            txtID.Enabled = false;
            txtID.BackColor = SystemColors.Control;
            txtNome.Enabled = true;
            txtBreve.Enabled = true;
            txtNacionalidade.Enabled = true;
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
            txtID.Enabled = false;
            txtID.BackColor = SystemColors.Control;
            txtNome.Enabled = false;
            txtBreve.Enabled = false;
            txtNacionalidade.Enabled = false;
            dtpData.Enabled = false;
            pbFoto.Enabled = false;

            estadoAtual = FormState.Visualizando;
        }

        private void LimparCampos()
        {
            txtID.Clear();
            txtNome.Clear();
            txtBreve.Clear();
            txtNacionalidade.Clear();
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
                MessageBox.Show("Preencha os dados do novo piloto. O ID será gerado automaticamente.", "Adicionar Piloto", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                dgvPilotos.Visible = true;
                CarregarTodosPilotos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao visualizar pilotos: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAtualizar_Click(object sender, EventArgs e)
        {
            try
            {
                string input = ShowInputDialog("Digite o ID do Piloto que deseja editar:", "Editar Pilotos");

                if (!string.IsNullOrEmpty(input) && int.TryParse(input, out int id))
                {
                    if (CarregarPiloto(id))
                    {
                        idOriginal = id;
                        ConfigurarEstadoEditar();
                        MessageBox.Show("Dados carregados. Faça as alterações e clique em Confirmar.", "Editar Piloto", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Piloto não encontrado(a)!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar Piloto para edição: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDeletar_Click(object sender, EventArgs e)
        {
            try
            {
                string input = ShowInputDialog("Digite o ID do Piloto que deseja deletar:", "Deletar Piloto");

                if (!string.IsNullOrEmpty(input) && int.TryParse(input, out int id))
                {
                    if (DeletarPiloto(id))
                    {
                        MessageBox.Show("Piloto deletado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ConfigurarEstadoInicial();
                        if (dgvPilotos.Visible)
                        {
                            CarregarTodosPilotos();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao deletar Piloto: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    if (AdicionarPiloto())
                    {
                        MessageBox.Show("Piloto adicionado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ConfigurarEstadoInicial();
                        if (dgvPilotos.Visible)
                        {
                            CarregarTodosPilotos();
                        }
                    }
                }
                else if (estadoAtual == FormState.Editando)
                {
                    if (AtualizarPiloto())
                    {
                        MessageBox.Show("Piloto atualizado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        btnSalvar.Text = "Salvar";
                        ConfigurarEstadoInicial();
                        if (dgvPilotos.Visible)
                        {
                            CarregarTodosPilotos();
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
                    openFileDialog.Title = "Selecionar Foto do Piloto";

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
        #endregion

        #region Validação
        private bool ValidarCampos()
        {

            if (string.IsNullOrWhiteSpace(txtNome.Text))
            {
                MessageBox.Show("O campo Nome é obrigatório!", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNome.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtBreve.Text))
            {
                MessageBox.Show("O campo Brevê é obrigatório!", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBreve.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNacionalidade.Text))
            {
                MessageBox.Show("O campo Nacionalidade é obrigatório!", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNacionalidade.Focus();
                return false;
            }

            return true;
        }

        #endregion

        #region Operações CRUD
        private bool AdicionarPiloto()
        {
            try
            {
                var newRow = dataset.Piloto.NewPilotoRow();
                newRow.Nome = txtNome.Text.Trim();
                newRow.Breve = txtBreve.Text.Trim();
                newRow.Nacionalidade = txtNacionalidade.Text.Trim();
                newRow.DataNasc = dtpData.Value;

                if (!string.IsNullOrEmpty(caminhoImagemSelecionada))
                {
                    newRow.Foto = caminhoImagemSelecionada;
                }
                else
                {
                    newRow.Foto = string.Empty;
                }

                dataset.Piloto.AddPilotoRow(newRow);
                pilotoTableAdapter.Update(dataset.Piloto);

                int novoId = newRow.Id;
                MessageBox.Show($"Piloto adicionado com sucesso!\nID gerado automaticamente: {novoId}", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao adicionar piloto: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool CarregarPiloto(int id)
        {
            try
            {
                dataset.Piloto.Clear();
                pilotoTableAdapter.Fill(dataset.Piloto);

                var piloto = dataset.Piloto.AsEnumerable().FirstOrDefault(p => p.Field<int>("Id") == id);

                if (piloto != null)
                {
                    txtID.Text = piloto.Field<int>("Id").ToString();
                    txtNome.Text = piloto.Field<string>("Nome") ?? "";
                    txtBreve.Text = piloto.Field<string>("Breve") ?? "";
                    txtNacionalidade.Text = piloto.Field<string>("Nacionalidade") ?? "";
                    dtpData.Value = piloto.Field<DateTime>("DataNasc");

                    string caminhoFoto = piloto.Field<string>("Foto");
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
                MessageBox.Show($"Erro ao carregar piloto: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void CarregarTodosPilotos()
        {
            try
            {
                dataset.Piloto.Clear();
                pilotoTableAdapter.Fill(dataset.Piloto);

                dtPilotos.Rows.Clear();

                if (dataset.Piloto.Rows.Count == 0)
                {
                    MessageBox.Show("Nenhum piloto cadastrado no sistema.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                foreach (DataRow row in dataset.Piloto.Rows)
                {
                    int id = Convert.ToInt32(row["Id"]);
                    string nome = row["Nome"]?.ToString() ?? "N/A";
                    string breve = row["Breve"]?.ToString() ?? "N/A";
                    string nacionalidade = row["Nacionalidade"]?.ToString() ?? "N/A";
                    DateTime dataNasc = Convert.ToDateTime(row["DataNasc"]);

                    int idade = DateTime.Today.Year - dataNasc.Year;
                    if (dataNasc.Date > DateTime.Today.AddYears(-idade)) idade--;

                    dtPilotos.Rows.Add(id, nome, breve, nacionalidade, dataNasc, idade);
                }

                dgvPilotos.ReadOnly = true;
                dgvPilotos.AllowUserToAddRows = false;
                dgvPilotos.AllowUserToDeleteRows = false;

                dgvPilotos.Refresh();
                ConfigurarEstadoVisualizar();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar lista de pilotos: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool AtualizarPiloto()
        {
            try
            {
                if (idOriginal == -1)
                {
                    MessageBox.Show("Erro: ID original não foi definido!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                string nome = txtNome.Text.Trim();
                string breve = txtBreve.Text.Trim();
                string nacionalidade = txtNacionalidade.Text.Trim();
                DateTime dataNasc = dtpData.Value;

                string caminhoFoto = caminhoImagemSelecionada;

                if (string.IsNullOrEmpty(caminhoFoto))
                {
                    dataset.Piloto.Clear();
                    pilotoTableAdapter.Fill(dataset.Piloto);
                    var pilotoAtual = dataset.Piloto.AsEnumerable().FirstOrDefault(p => p.Field<int>("Id") == idOriginal);
                    if (pilotoAtual != null)
                    {
                        caminhoFoto = pilotoAtual.Field<string>("Foto") ?? string.Empty;
                    }
                }

                var rowToUpdate = dataset.Piloto.AsEnumerable().FirstOrDefault(p => p.Field<int>("Id") == idOriginal);
                if (rowToUpdate != null)
                {
                    rowToUpdate["Nome"] = nome;
                    rowToUpdate["Breve"] = breve;
                    rowToUpdate["Nacionalidade"] = nacionalidade;
                    rowToUpdate["DataNasc"] = dataNasc;
                    rowToUpdate["Foto"] = caminhoFoto;

                    pilotoTableAdapter.Update(dataset.Piloto);

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao atualizar Piloto: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool DeletarPiloto(int id)
        {
            try
            {
                dataset.Piloto.Clear();
                pilotoTableAdapter.Fill(dataset.Piloto);
                var piloto = dataset.Piloto.AsEnumerable().FirstOrDefault(p => p.Field<int>("Id") == id);

                if (piloto == null)
                {
                    MessageBox.Show("Piloto não encontrado!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                string nomePiloto = piloto.Field<string>("Nome") ?? "[Nome não disponível]";

                if (MessageBox.Show($"Tem certeza que deseja deletar o piloto:\n\nID: {id}\nNome: {nomePiloto}",
                    "Confirmar Exclusão", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var rowToDelete = dataset.Piloto.AsEnumerable().FirstOrDefault(p => p.Field<int>("Id") == id);
                    if (rowToDelete != null)
                    {
                        rowToDelete.Delete();

                        pilotoTableAdapter.Update(dataset.Piloto);

                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao deletar piloto: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        #endregion

        #region Eventos do DataGridView
        private void dgvPilotos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvPilotos_SelectionChanged(object sender, EventArgs e)
        {

        }

        private void CarregarImagemPiloto(int idPiloto)
        {
            try
            {
                dataset.Piloto.Clear();
                pilotoTableAdapter.Fill(dataset.Piloto);

                var piloto = dataset.Piloto.AsEnumerable()
                               .FirstOrDefault(p => p.Field<int>("Id") == idPiloto);

                if (piloto != null)
                {
                    string caminhoFoto = piloto.Field<string>("Foto");
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
                MessageBox.Show($"Erro ao carregar imagem: {ex.Message}", "Erro",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                pbFoto.Image = null;
            }
        }

        private void dgvPilotos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    string nomeColuna = dgvPilotos.Columns[e.ColumnIndex].DataPropertyName;

                    if (nomeColuna.Equals("ID", StringComparison.OrdinalIgnoreCase))
                    {
                        int idPiloto = Convert.ToInt32(dgvPilotos.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);

                        CarregarImagemPiloto(idPiloto);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar piloto: {ex.Message}", "Erro",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Outros Eventos
        private void dtpData_ValueChanged(object sender, EventArgs e)
        {
            if (dtpData.Value > DateTime.Now)
            {
                MessageBox.Show("A data de nascimento não pode ser futura!", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private void txtID_TextChanged(object sender, EventArgs e)
        {
            // Método 
        }

        private void txtNome_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtNome.Text))
            {
                txtNome.ForeColor = Color.Red;
            }
            else
            {
                txtNome.ForeColor = SystemColors.WindowText;
            }
        }

        private void txtBreve_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBreve.Text))
            {
                txtBreve.ForeColor = Color.Red;
            }
            else
            {
                txtBreve.ForeColor = SystemColors.WindowText;
            }
        }

        private void txtNacionalidade_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtNacionalidade.Text))
            {
                txtNacionalidade.ForeColor = Color.Red;
            }
            else
            {
                txtNacionalidade.ForeColor = SystemColors.WindowText;
            }
        }
        #endregion

        #region Carregamento do Form
        private void Pilotos_gen_Load(object sender, EventArgs e)
        {
            try
            {
                pilotoTableAdapter.Fill(dataset.Piloto);
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

        private void txtID_TextChanged_1(object sender, EventArgs e)
        {
        }

        private void Pilotos_gen_Load_1(object sender, EventArgs e)
        {
        }
    }
}