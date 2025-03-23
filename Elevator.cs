using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TpCalculette
{
    public partial class Ascenseur : Form
    {

        private int etageActuel = 1;
        private bool estEnMouvement = false;
        private bool[] demandes = new bool[4];
        private bool[] demandesEnAttente = new bool[8];
        private List<int> fileAttente = new List<int>();
        public Ascenseur()
        {
            InitializeComponent();
            pictureBoxFermer.Visible = true;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(510, 550);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }





        private async void DeplacerAscenseur(int nouvelEtage)
        {
            if (etageActuel == nouvelEtage)
            {
                return;
            }

            if (estEnMouvement)
            {
                fileAttente.Add(nouvelEtage);
                return;
            }

            estEnMouvement = true;
            UpdatePictureBoxVisibility();
            buttonOuvrir.ForeColor = Color.Red;

            pictureBoxOvert.Visible = false;
            pictureBoxFermer.Visible = true;
            await Task.Delay(2000);

            int differenceEtages = nouvelEtage - etageActuel;
            int hauteurEtage = 120;
            int vitesse = 1;

            for (int i = 0; i < Math.Abs(differenceEtages) * hauteurEtage; i += vitesse)
            {
                int deplacementY = differenceEtages > 0 ? vitesse : -vitesse;
                groupBox1.Location = new Point(groupBox1.Location.X, groupBox1.Location.Y - deplacementY);
                await Task.Delay(10);

                pictureBoxOvert.Visible = false;
                pictureBoxFermer.Visible = true;

                if ((differenceEtages > 0 && groupBox1.Location.Y <= GetTreeViewLocationY(nouvelEtage)) ||
                    (differenceEtages < 0 && groupBox1.Location.Y >= GetTreeViewLocationY(nouvelEtage)))
                {
                    break;
                }
            }

            etageActuel = nouvelEtage;
            pictureBoxOvert.Visible = true;
            pictureBoxFermer.Visible = false;
            ResetButtonColor();
            UpdatePictureBoxVisibility();
            await Task.Delay(2000);

            estEnMouvement = false;

            if (fileAttente.Count > 0)
            {
                int prochaineDemande = fileAttente.First();
                fileAttente.RemoveAt(0);
                DeplacerAscenseur(prochaineDemande);
            }
        }
        public void AjouterFileAttente(int nouvelEtage)
        {
            fileAttente.Add(nouvelEtage);
        }

        public void DemandeEtageVide(int etage)
        {
            if (!estEnMouvement)
            {
                DeplacerAscenseur(etage);
            }
            else
            {
                AjouterFileAttente(etage);
            }
        }



        private void ResetButtonColor()
        {

            buttonOuvrir.ForeColor = Color.Lime;
        }


        private void UpdatePictureBoxVisibility()
        {
            if (etageActuel == 1 && estEnMouvement == false)
            {
                pictureBox1.Visible = false;
            }
            else
            {
                pictureBox1.Visible = true;
            }
            if (etageActuel == 2 && estEnMouvement == false)
            {
                pictureBox2.Visible = false;
            }
            else
            {
                pictureBox1.Visible = true;
            }
            if (etageActuel == 3 && estEnMouvement == false)
            {
                pictureBox3.Visible = false;
            }
            else
            {
                pictureBox1.Visible = true;
            }
            if (etageActuel == 4 && estEnMouvement == false)
            {
                pictureBox4.Visible = false;
            }
            else
            {
                pictureBox1.Visible = true;
            }
        }


        private int GetTreeViewLocationY(int etage)
        {
            switch (etage)
            {
                case 1:
                    return treeViewEtage2.Location.Y;
                case 2:
                    return treeViewEtage3.Location.Y;
                case 3:
                    return treeViewEtage4.Location.Y;
                default:
                    return 0;
            }
        }

       

        public void TraiterDemandesSimultanees(int[] etages)
        {
            if (!estEnMouvement)
            {
                if (etages.Length > 0)
                {
                    DeplacerAscenseur(etages[0]);
                }
            }
            else
            {
                foreach (var etage in etages)
                {
                    demandesEnAttente[etage - 1] = true;
                }
            }
        }

        public void DemandeInterieure(int nouvelEtage)
        {
            if (!estEnMouvement)
            {
                DeplacerAscenseur(nouvelEtage);
            }
            else
            {
                demandesEnAttente[nouvelEtage - 1] = true;
            }
        }

        public void TraiterDemandesSuccessives(int[] etages)
        {
            if (!estEnMouvement)
            {
                foreach (var etage in etages)
                {
                    DeplacerAscenseur(etage);
                }
            }
            else
            {
                foreach (var etage in etages)
                {
                    demandesEnAttente[etage - 1] = true;
                }
            }
        }

        public void TraiterDemandesDeGroupe(int etage)
        {
            if (!estEnMouvement)
            {
                DeplacerAscenseur(etage);
            }
            else
            {
                demandesEnAttente[etage - 1] = true;
            }
        }


        public void TraiterDemandesEnAttente()
        {
            int prochaineDemande = -1;
            for (int i = etageActuel - 1; i < demandesEnAttente.Length; i++)
            {
                if (demandesEnAttente[i])
                {
                    prochaineDemande = i + 1;
                    demandesEnAttente[i] = false;
                    break;
                }
            }

            if (prochaineDemande != -1)
            {
                DeplacerAscenseur(prochaineDemande);
            }
        }



        private void btnDemande1_Click(object sender, EventArgs e)
        {
            if (!estEnMouvement)
            {
                DeplacerAscenseur(1);
            }
            else
            {
                AjouterFileAttente(1);
            }
        }

        private void btnEtage1Click(object sender, EventArgs e)
        {

            DeplacerAscenseur(1);

        }
        private void btnEtage2Click(object sender, EventArgs e)
        {
           
                DeplacerAscenseur(2);
       
        }

        private void btnEtage3Click(object sender, EventArgs e)
        {
            
                DeplacerAscenseur(3);
            
           
            
        }

        private void btnEtage4Click(object sender, EventArgs e)
        {
            
                DeplacerAscenseur(4);
          
        }

        private void btnDemande2_Click(object sender, EventArgs e)
        {
            if (!estEnMouvement)
            {
                DeplacerAscenseur(2);
            }
            else
            {
                AjouterFileAttente(2);
            }
        }

        private void btnDemande3_Click(object sender, EventArgs e)
        {
            if (!estEnMouvement)
            {
                DeplacerAscenseur(3);
            }
            else
            {
                AjouterFileAttente(3);
            }
        }

        private void btnDemande4_Click(object sender, EventArgs e)
        {
            if (!estEnMouvement)
            {
                DeplacerAscenseur(4);
            }
            else
            {
                AjouterFileAttente(4);
            }
        }


        private void buttonOuvrir_Click(object sender, EventArgs e)
        {

            if (!estEnMouvement)
            {

                pictureBoxOvert.Visible = true;
                pictureBoxFermer.Visible = false;

            }
            else
            {


            }

        }

        private void buttonFermer_Click(object sender, EventArgs e)
        {
            pictureBoxOvert.Visible = false;


            pictureBoxFermer.Visible = true;




        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }



        private void button12_Click(object sender, EventArgs e)
        {

        }

        private void button11_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }


        private void pictureBoxOvert_Click_1(object sender, EventArgs e)
        {

        }

        private void pictureBoxfermer_Click_1(object sender, EventArgs e)
        {

        }

        private void treeViewEtage2_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void treeViewEtage3_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void treeViewEtage4_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click_1(object sender, EventArgs e)
        {

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {

        }

    }
}
