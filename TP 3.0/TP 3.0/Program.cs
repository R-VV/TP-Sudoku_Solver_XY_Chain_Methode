using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace TP_3._0
{
    class Program
    {

        public static ArrayList[] Lecture()
        {
            ArrayList liste_quizz = new ArrayList();
            ArrayList liste_solution = new ArrayList();

            string nomFich = "C:\\Users\\Romuald\\Desktop\\ESGF M2\\Big Data Cloud computing\\sudoku.txt ";
            StreamReader fichLect = new StreamReader(nomFich);
            string ligne = "";
            while (fichLect.Peek() > 0)
            {
                ligne = fichLect.ReadLine();
                string[] ligne_lue = ligne.Split(",");
                liste_quizz.Add(ligne_lue[0]);
                liste_solution.Add(ligne_lue[1]);
            }
            fichLect.Close();
            ArrayList[] liste_d_array = new ArrayList[2];
            liste_d_array[0] = liste_quizz;
            liste_d_array[1] = liste_solution;

            return liste_d_array;
        }

        static void Affiche_Sudoku(string chaine)
        {
            int i = 0;
            for (int l = 0; l < 9; l++)
            {
                if (l % 3 == 0) Console.WriteLine("--------------------");
                for (int c = 0; c < 9; c++)
                {
                    if (c % 3 == 0) Console.Write("|");
                    Console.Write(chaine[i] + " ");i++;
                }
                Console.Write("\n");
            }
        }

        static List<List<int>> Stocker_val_possible(List<Cellule> list_cell)
        {
            List<List<int>> Liste_sauv_val_possible = new List<List<int>>();
            foreach (Cellule cell in list_cell)
                Liste_sauv_val_possible.Add(cell.valeurs_possibles);
            return Liste_sauv_val_possible;
        }

        static void Main(string[] args)
        {
            //N'ayant pas réussi à "Deep Cloner" l'objet Sudoku (dépendance observée) pour effectuer des tests sans modifier le sudoku original,
            //une méthode alternative, par enregistrement dans plusieurs listes des données de la matrice sudoku originale puis re-création
            //de la matrice originale depuis ces listes à chaque "tour",a été utilisée

            Console.WriteLine("Indiquer le numéro du sudoku souhaité (entre 1 et 1 000 000)");
            int num = Convert.ToInt32(Console.ReadLine());
            string chaine = ((string)Lecture()[0][num]);
            Console.WriteLine("Sudoku vierge demandé : ");
            Affiche_Sudoku(chaine);

            Console.WriteLine("\nSa correction selon le fichier");
            string chaine2 = ((string)Lecture()[1][num]);
            Affiche_Sudoku(chaine2); Console.Write("\nSa correction après traitement par l'algorithme");

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            List<Cellule> Liste0_Cell = new List<Cellule>();
            List<Cellule> Liste_Historique_Cell = new List<Cellule>();
            List<List<int>> Liste0_Int = new List<List<int>>();
            List<List<int>> Liste_Historique_Int = new List<List<int>>();
            int nb_blocage = 0; bool blocage = false;
            Sudoku s1 = new Sudoku();

            while (blocage == false)
            {
                for (int L = 0; L < 9; L++)
                {
                    for (int C = 0; C < 9; C++)
                    {
                        for (int f = 0; f < 2; f++)
                        {
                            s1.Set_matrice(chaine);
                            s1.Reset();
                            s1.Mise_à_jour(Liste_Historique_Cell, Liste_Historique_Int);
                            s1.Ajouter_val_possible();
                            Cellule cell_cible = s1.Cell(L, C);

                            if (cell_cible.valeur_initiale == 0 && cell_cible.valeurs_possibles.Count == 2)
                            {
                                nb_blocage = 0;
                                if (f == 1) cell_cible.valeurs_possibles.Reverse();
                                Liste0_Cell = s1.Retourne_Liste0_Cell(cell_cible);

                                //Profondeur 10 arbitraire choisie
                                s1.Simulation(10, cell_cible);

                                Liste0_Int = Stocker_val_possible(Liste0_Cell);
                                Liste_Historique_Cell.AddRange(Liste0_Cell);
                                Liste_Historique_Int.AddRange(Liste0_Int);
                            }
                            else { nb_blocage += 1; }                           
                            if (nb_blocage > 81) blocage = true;
                        }
                    }
                    s1.Tests_triviaux();
                }
            }
            s1.Mise_à_jour2();
            s1.Remplissage_par_test_aléatoire();
            s1.Affiche_Sudoku();

            stopWatch.Stop();

            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            Console.WriteLine("Durée exécution code résolution " + elapsedTime);





        }
    }
}
