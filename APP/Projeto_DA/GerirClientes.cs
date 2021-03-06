﻿using Projeto_DA;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Home
{
    public partial class GerirClientes : Form
    {
        //perparar o container para a base de dados
        private masterEntities imobiliaria;
        //Perparar a class cliente para guardar o cliente selecionado
        private Cliente clienteSelecionado;
        //Novo cliente
        private bool novo = false;

        //Inicio do form 2 
        public GerirClientes(masterEntities imobiliaria)
        {
            //Inicia os componentes do form2 
            InitializeComponent();
            //Atualizar a lista dos clientes sem nenhuma excecao

            //imobiliaria = new masterEntities();
            this.imobiliaria = imobiliaria;
            atualizarListaClientes();
            //Meter o cliente selecionado a null

            clienteSelecionado = null;
            cbfilter_type.SelectedIndex = 0;
            if (imobiliaria.Clientes.Local.Count() == 0)
                limpar_campos();

        }

        //Butao de filtro quando sera percionado 
        private void btnFilter_Click(object sender, EventArgs e)
        {
            //Chamar a funcao para filtrar
            if (txtNome_Filter.Text != string.Empty)
                filter_Name(cbfilter_type.SelectedIndex);
            else
                MessageBox.Show("Nao tem nada para pesquisar!", "Caixa de Texto Vazia", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        //Funcao executa quando o texto da mesma tb muda 
        private void txtNome_Filter_TextChanged(object sender, EventArgs e)
        {
            //Chamar a funcao para filtrar
            if (txtNome_Filter.Text != string.Empty)
                filter_Name(cbfilter_type.SelectedIndex);
            else
                MessageBox.Show("Nao tem nada para pesquisar!", "Caixa de Texto Vazia", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        //Quando o este form for fechado
        private void GerirClientes_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Hide();
            //Perparar para abrir o form 1 
            Home frm = new Home();
            //Mostar o form 1
            frm.ShowDialog();
            //Fechar o form 
            this.Close();
        }

        //Quando o botao de save for precionado esta funcao sera ativada
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                //Verificar se existe algum cliente selecionado para saber se atualiza os dados ou eh um novo registo
                if (clienteSelecionado != null)
                {
                    //Chamar a funcao para atualizar os clientes 
                    if (verificar() == true && novo == false)
                    {
                        atualizar_Clientes();
                    }
                }

                //Chamar a funcao para atualizar a lista dos clientes
                atualizarListaClientes();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ops Samething went wrong!!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //Funcao executa quando a selecao da data grid view muda
        private void clienteDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                DataGridViewRow current = clienteDataGridView.CurrentRow;
                if (current != null) // Means that you've not clicked the column header
                {
                    //Para ir buscar o cliente que foi selecionado
                    clienteSelecionado = (Cliente)clienteDataGridView.CurrentRow.DataBoundItem;
                    if (clienteSelecionado != null)
                    {
                        //Mete os dados no seu citio de acordo com o cliente selecionado
                        txtNome.Text = clienteSelecionado.Nome;
                        txtNif.Text = clienteSelecionado.Nif;
                        txtMorada.Text = clienteSelecionado.Morada;
                        txtContacto.Text = clienteSelecionado.Contacto;
                        //Bloquear Butao novo
                        btnNovo.Enabled = false;
                        //Desbloquear butao
                        btnRemover.Enabled = true;
                        novo = false;
                    }
                    //Chamar a funcao para atualizar as lista das casas
                    atualiza_Casas();
                    atualizar_Arrendamentos();
                    atualizar_Aquisicoes();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ops Samething went wrong!!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        //quando o butao novo eh precionadado
        private void btnNovo_Click(object sender, EventArgs e)
        {
            try
            {
                //Limpar selecionados
                clienteSelecionado = null;
                //Chamar a funcao para introdudizir os dados na base de dados;
                if (verificar() == true)
                {
                    adicionar_Clientes();
                }
                novo = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ops Samething went wrong!!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        //Limpar os campos todos
        private void limpar_campos()
        {
            //Limpar selecionados
            clienteSelecionado = null;
            //Descelecionar listas de gird view 
            clienteDataGridView.ClearSelection();
            lb_Casas.DataSource = null;
            lb_Aquisicoes.DataSource = null;
            lb_Arrendamentos.DataSource = null;
            //Limpar caixas de texto
            txtNome.Text = string.Empty;
            txtNif.Text = string.Empty;
            txtMorada.Text = string.Empty;
            txtContacto.Text = string.Empty;
            //Ativar butoes
            btnNovo.Enabled = true;
            //Desativar butoes
            btnRemover.Enabled = false;
            //Meter o novo igual a a true
            novo = true;
        }

        //Filtrar a data grid view pelo nome que foi metido na text box
        private void filter_Name(int type)
        {
            try
            {
                //Verifica se a text box tem alguma cena escrita la dentro
                if (txtNome_Filter.Text.Length > 0)
                {
                    //desativa o botao de adicionar novo item para a base de dados
                    btnGuardar.Enabled = false;
                    //para dar dispose da imobiliaria 
                    ///
                    ///Sem a proxima linha de codigo isto nao funciona
                    ///
                    imobiliaria.Dispose();
                    //renovar o container
                    imobiliaria = new masterEntities();
                    //selecionar o conteudo da base de dados de acordo com o que foi pedido pela text de filtro
                    // MessageBox.Show(type.ToString());
                    if (type == 0)
                    {
                        (from cliente in imobiliaria.Clientes
                         where cliente.Nome.ToUpper().Contains(txtNome_Filter.Text.ToUpper())
                         orderby cliente.Nome
                         select cliente).ToList();
                        //Carregar a informaçao pedida acima para a list box
                        clienteBindingSource.DataSource = imobiliaria.Clientes.Local.ToBindingList();
                    }
                    else if (type == 1)
                    {
                        //Marca a imobiliaria como novo container da base de dados 
                        //imobiliaria = new masterEntities();
                        (from cliente in imobiliaria.Clientes
                         where cliente.Nif.ToUpper().Contains(txtNome_Filter.Text.ToUpper())
                         orderby cliente.Nif
                         select cliente).ToList();
                        //Carregar a informaçao pedida acima para a list box
                        clienteBindingSource.DataSource = imobiliaria.Clientes.Local.ToBindingList();
                    }
                    else
                    {
                        //Marca a imobiliaria como novo container da base de dados 
                        //imobiliaria = new masterEntities();
                        (from cliente in imobiliaria.Clientes
                         where cliente.Contacto.ToUpper().Contains(txtNome_Filter.Text.ToUpper())
                         orderby cliente.Contacto
                         select cliente).ToList();
                        //Carregar a informaçao pedida acima para a list box
                        clienteBindingSource.DataSource = imobiliaria.Clientes.Local.ToBindingList();
                    }
                }
                //se nao tiver texto
                else
                {
                    //o botao de adicionar novo item estara ent ativo
                    btnGuardar.Enabled = true;
                    //para dar dispose da imobiliaria
                    imobiliaria.Dispose();
                    //Atualizar a lista dos clientes sem nenhuma excecao
                    atualizarListaClientes();
                }
            }
            catch (Exception ex)
            {
                //messagem de erro
                MessageBox.Show(ex.ToString(), "Ops Samething went wrong!!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        //Funcao para adicionar novo cliente
        private void adicionar_Clientes()
        {
            try
            {
                //Adicionar novo cliente a class
                Cliente clienteTemp = new Cliente();
                clienteTemp.Nome = txtNome.Text;
                clienteTemp.Nif = txtNif.Text;
                clienteTemp.Morada = txtMorada.Text;
                clienteTemp.Contacto = txtContacto.Text;
                //Adiconar o cliente da class a base de dados
                imobiliaria.Clientes.Add(clienteTemp);
                //Processo de guardados na base de dados
                imobiliaria.SaveChanges();
            }
            catch (Exception ex)
            {
                //messagem de erro
                MessageBox.Show(ex.ToString(), "Ops Samething went wrong!!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        //Atualizar a data grid view sem excesoes
        private void atualizarListaClientes()
        {
            try
            {
                //Seleciona o conteudo da base de dados e organiza - o por nome 
                (from cliente in imobiliaria.Clientes orderby cliente.Nome select cliente).Load();
                //Carrega a informaçao que foi pedida na linha anterior para a listbox que foi gerada
                clienteBindingSource.DataSource = imobiliaria.Clientes.Local.ToBindingList();
            }
            catch (Exception ex)
            {
                //messagem de erro
                MessageBox.Show(ex.ToString(), "Ops Samething went wrong!!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //Quando o butao de remover for precionado
        private void btnRemover_Click(object sender, EventArgs e)
        {
            try
            {
                //Dialogo de messagem para perguntar se o deseja mesmo fechar o programa ou nao 
                DialogResult result = MessageBox.Show("Deseja eliminar este cliente?", "Eliminar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                //Verifica qual foi a opecao escolhida e se for a opcao nao este ira entrar no if 
                if (result == DialogResult.Yes)
                {
                    //Verificar se o cliente tem ou nao casas associadas
                    if (clienteSelecionado.Casas.Count() == 0 && clienteSelecionado.Aquisicoes.Count() == 0 && clienteSelecionado.Arrendamentos.Count() == 0)
                    {
                        //Chamar a funcao para eliminar os clientes
                        eliminiar_Clientes();
                        //Chamar a funcao para atualizar a lista dos clientes
                        atualizarListaClientes();
                    }
                    else
                        //messagem de erro
                        MessageBox.Show("O cliente ainda tem casas ou aquisicoes ou arrendamentos por eliminar!\nTem que eleminar tudo primeiro!\nSó depois é que pode eleminar o cliente!", "Eliminar Clientes?", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
            }
            catch (Exception ex)
            {
                //messagem de erro
                MessageBox.Show(ex.ToString(), "Ops Samething went wrong!!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        //Funcao para eliminar os clientes
        private void eliminiar_Clientes()
        {
            try
            {
                //Remover o cliente selecionado
                imobiliaria.Clientes.Remove(clienteSelecionado);
                //Processo de guardados na base de dados 
                imobiliaria.SaveChanges();
                //Chamnar a funcao para limpar os campos todos
                limpar_campos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ops Samething went wrong!!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //Funcao para atualizar os clientes
        private void atualizar_Clientes()
        {
            try
            {
                //Atualizar os dados dos clientes
                clienteSelecionado.Nome = txtNome.Text;
                clienteSelecionado.Nif = txtNif.Text;
                clienteSelecionado.Morada = txtMorada.Text;
                clienteSelecionado.Contacto = txtContacto.Text;

                //Processo de guardados na base de dados 
                imobiliaria.SaveChanges();
            }
            catch (Exception ex)
            {
                //messagem de erro
                MessageBox.Show(ex.ToString(), "Ops Samething went wrong!!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //Limpar campos
        private void btnClear_Click(object sender, EventArgs e)
        {
            //Limpar campos
            limpar_campos();
        }

        //verificar se as texts boxs estao vazias
        private bool verificar()
        {
            bool status = true;
            if (txtNome.Text == string.Empty)
            {
                MessageBox.Show("O campo <NOME> não pode estar vazio!", "Adiconar Cliente", MessageBoxButtons.OK, MessageBoxIcon.Information);
                status = false;
            }
            else if (txtNif.Text == string.Empty)
            {
                MessageBox.Show("O campo <NIF> não pode estar vazio!", "Adiconar Cliente", MessageBoxButtons.OK, MessageBoxIcon.Information);
                status = false;
            }
            else if (txtMorada.Text == string.Empty)
            {
                MessageBox.Show("O campo <MORADA> não pode estar vazio!", "Adiconar Cliente", MessageBoxButtons.OK, MessageBoxIcon.Information);
                status = false;
            }
            else if (txtContacto.Text == string.Empty)
            {
                MessageBox.Show("O campo <CONTACTO> não pode estar vazio!", "Adiconar Cliente", MessageBoxButtons.OK, MessageBoxIcon.Information);
                status = false;
            }
            return status;
        }

        //Fazer o butao guardar mais gostoso
        private void timer_Tick(object sender, EventArgs e)
        {
            if (true)
            {
                String.IsNullOrEmpty(txtContacto.Text);
            }
            //Verifica se os campos estao preechidos
            if (txtNome.Text == string.Empty)
                btnGuardar.Enabled = false;
            else if (txtNif.Text == string.Empty)
                btnGuardar.Enabled = false;
            else if (txtMorada.Text == string.Empty)
                btnGuardar.Enabled = false;
            else if (txtContacto.Text == string.Empty)
                btnGuardar.Enabled = false;
            else if (novo == true)
                btnGuardar.Enabled = false;
            else
                btnGuardar.Enabled = true;

        }

        //Quando uma key for precionada na textbox contacto
        private void txtContacto_KeyPress(object sender, KeyPressEventArgs e)
        {
            //verifica se a key precionada eh um numero ou nao
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        //Atualizar a lista de casas
        private void atualiza_Casas()
        {
            //verificar se existe
            if (clienteSelecionado != null)
            {
                lb_Casas.DataSource = null;
                lb_Casas.DataSource = clienteSelecionado.Casas.ToList<Casa>();
                /* if (clienteSelecionado.Casas is CasaArrendavel)
                 {
                     lb_Casas.DataSource = clienteSelecionado.Casas.ToList<CasaArrendavel>();
                 }
                 if (clienteSelecionado.Casas is CasaVendavel)
                 {
                     lb_Casas.DataSource = clienteSelecionado.Casas.ToList<CasaVendavel>();
                 }*/
            }
        }

        //Quando uma key for precionada na textbox nif
        private void txtNif_KeyPress(object sender, KeyPressEventArgs e)
        {
            //verifica se a key precionada eh um numero ou nao
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        //funcao para atualizar os arrendamentos
        private void atualizar_Arrendamentos()
        {
            if (clienteSelecionado != null)
            {
                lb_Arrendamentos.DataSource = null;
                lb_Arrendamentos.DataSource = clienteSelecionado.Arrendamentos.ToList<Arrendamento>();
            }
        }

        //Funcao para atualizar as aquisiçoes
        private void atualizar_Aquisicoes()
        {
            if (clienteSelecionado != null)
            {
                lb_Aquisicoes.DataSource = null;
                lb_Aquisicoes.DataSource = clienteSelecionado.Aquisicoes.ToList<Venda>();
            }
        }
    }
}
