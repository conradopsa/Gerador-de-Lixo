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
using System.Threading;

namespace Gerar_Lixo
{
    public partial class Form1 : Form
    {
        //Desenvolvido por Supreme Developer (ConradoPSA)

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog SFD = new SaveFileDialog();

            if (SFD.ShowDialog() == DialogResult.OK)
            {
                int qtd = Convert.ToInt32(numericUpDown1.Value);                
                int cbIndex = comboBox1.SelectedIndex;
                FileStream FS = File.OpenWrite(SFD.FileName);

                /*Invoca a Função Escrever em uma nova Thread
                 para que o form principal não fique inativo durante
                 o processo*/
                Thread th = new Thread(() => Gerar(FS, qtd, cbIndex));
                th.Start();

            }
        }

        const string msgboxTitulo = "Gerador de Lixo";
        private void Gerar(FileStream FS, int val, int index)
        {
            progressBar1.Value = 0;

            //index: b = 0, kb = 1, mb = 2, gb = 3, tb = 4
            //qtd = val  * 1024 ^ index
            Int64 qtd = val * (Int64)Math.Pow(1024.0, index);

            #region Curiosidade
            //1 Terabyte = 1.099.511.627.776 bytes
            //Valor máximo do Int64 = 9.223.372.036.854.775.807
            //Portanto, limite de geração de lixo ~= 8.388.607,99 Terabytes
            #endregion

            try
            {
                for (Int64 i = 1; i <= qtd; i++)
                {
                    FS.WriteByte((byte)0);

                    //Atualiza a cada 104857600 repetições (100 Mb) ou na última repetição
                    if (i % 104857600 == 0 || i == qtd)
                    {
                        FS.Flush();

                        //Invoca a thread principal e executa ações
                        progressBar1.Invoke(new Action(() => {
                            progressBar1.Value = (int)((i * 1.0f / qtd) * 100);

                            if (progressBar1.Value == 100)
                            {
                                MessageBox.Show("Lixo Gerado!", msgboxTitulo,
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                                progressBar1.Value = 0;
                            }

                        }));

                    }
                }

                FS.Dispose();
            }
            catch (IOException)
            {
                //Exceção gerada ao não ter mais espaço disponível

                progressBar1.Invoke(new Action(() => {
                    progressBar1.Value = 100;

                    MessageBox.Show("Espaço completamente preenchido!", msgboxTitulo,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    MessageBox.Show("Lixo Gerado!", msgboxTitulo,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    progressBar1.Value = 0;
                                       
                }));
            }
            

            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Padrão
            comboBox1.Text = "Kb";
        }
    }
}
