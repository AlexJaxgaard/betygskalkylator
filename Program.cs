using System;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic.FileIO;
using System.Globalization;
using CsvHelper;
using System.Text;


namespace betygskalkylator
{
    class Program
    {
        static void Main()
        {
            //definerar variabler 
            int x = 0;
            double totalpoäng = 0;
            bool gymnasiebehörighet;

            /*ÄNDRA CSV-PATH PÅ LINJE 27 OCH EXPORT-PATH PÅ LINJE 347 FÖR ATT PROGRAMMET SKA KUNNA FORTSÄTTA*/

            Console.WriteLine("Läser in CSV...");


            //Definerar vilken csv-fil som används i programmet
            var csv = @"/Users/scandinaviana/Desktop/grade_calc_1.csv";
            //Kollar om ovan är tomt
            if (csv == null)
            {
                Console.WriteLine("Filen som du skrivit in är inte giltig, försök igen.");
                return;

            }
            //Kollar om filen existerar eller inte
            else if ((!File.Exists(csv)))
            {
                Console.WriteLine("Filen som du angett existerar inte. Programmet avbryts.");
                Console.WriteLine("ERR: File does not exist. Thrown at Line38");
                Console.WriteLine("");
                return;
            }
            else
            {


                Console.WriteLine("Använder " + csv + " som fil.");
                //Anropar funktion för att definera double[] betyg
                double[] betyg = Omräknare(csv);

                int arrayLängd = betyg.Length;

                double maxmerit = betyg.Length * 20;

                Console.WriteLine(betyg[0]);
                //Adderar meritvärden till en totalpoäng
                while (x < arrayLängd)
                {
                    totalpoäng += betyg[x];
                    x++;
                }

                int[] positionSvenskaEngelska = BetygSvenskaEngelska(csv);
                int positionSvenska = (positionSvenskaEngelska[1] - 1);
                int positionEngelska = (positionSvenskaEngelska[0] - 1);
 
                

                //Loop som kollar på vilken nivå du har/inte har gymnasiebehörighet

                if (totalpoäng > 89 && betyg[positionSvenska] > 9 && betyg[positionEngelska] > 9)
                {
                    gymnasiebehörighet = true;
                } else
                {
                    gymnasiebehörighet = false;
                }
                
                if (betyg[0] == -1)
                {
                    Console.WriteLine("Något gick fel.");
                }
                else
                {

                    Console.WriteLine("Klar!");
                    Console.WriteLine("");
                    Console.WriteLine("Utifrån informationen som du angett så går du " + betyg.Length + " kurser.");
                    Console.WriteLine("Detta ger dig ett maxbetyg på " + maxmerit + " poäng.");
                    Console.WriteLine("Din meritpoäng just nu ligger på " + totalpoäng + " poäng.");
                    Console.WriteLine("");

                }
                
                if (gymnasiebehörighet)
                {
                    Console.WriteLine("Du har just nu gymnasiebehörighet. Detta innebär att du har godkänt i fler än 9 kurser, varav två är Engelska och Svenska.");
                }
                else
                {
                    Console.WriteLine("Du har för närvarande INTE gymnasiebehörighet eftersom: ");
                    if (totalpoäng < 90)
                    {
                        Console.WriteLine("*Du har under 90 meritpoäng.");
                    }
                    if (betyg[positionSvenska] < 10)
                    {
                        Console.WriteLine("*Du inte har godkänt i Svenska.");
                    }
                    if (betyg[positionEngelska] < 10)
                    {
                        Console.WriteLine("*Du inte har godkänt i Engelska.");
                    }
                }

                Console.WriteLine("");
                Console.WriteLine("Räknar om betyg till Amerikanska GPA..");

                int y = 0;
                //Skapar en ny array med längden "arrayLängd" som defineras lite högre upp
                double[] amerikanskaBetyg = new double[arrayLängd];

                //Bestämmer vad varje betyg är lika med i amerikanska betyg.
                while (y < arrayLängd)
                {
                    if (betyg[y] == 20.0)
                    {
                        amerikanskaBetyg[y] = 4.0;

                    }
                    else if (betyg[y] == 17.5)
                    {
                        amerikanskaBetyg[y] = 4.0;
                    }
                    else if (betyg[y] == 15.0)
                    {
                        amerikanskaBetyg[y] = 3.0;
                    }
                    else if (betyg[y] == 12.5)
                    {
                        amerikanskaBetyg[y] = 3.0;
                    }
                    else if (betyg[y] == 10.0)
                    {
                        amerikanskaBetyg[y] = 2.0;
                    }
                    else if (betyg[y] == 0.0)
                    {
                        amerikanskaBetyg[y] = 0.0;
                    }
                    y++;
                }


                int z = 0;
                double totalpoängAmerikanska = 0;
                //adderar varje poäng för sig 
                while (z < arrayLängd)
                {
                    totalpoängAmerikanska += amerikanskaBetyg[z];
                    z++;
                }
                //Delar totalpoängAmerikanska på längden av array för att få fram medelvärde (GPA)
                double totalpoängAmerikanskaDelad = totalpoängAmerikanska / amerikanskaBetyg.Length;



                Console.WriteLine("Ditt meritvärde omräknat till GPA är: " + totalpoängAmerikanskaDelad);

                if (totalpoängAmerikanskaDelad > 4.0)
                {
                    Console.WriteLine("Ditt GPA indikerar att dina betyg till stor del består av A. För att komma in på universitet/högre utbildning i Amerika spelar GPA stor roll,");
                    Console.WriteLine("men ännu större vikt läggs på vilka individuella betyg.");
                } else if (totalpoängAmerikanskaDelad < 4.0 && totalpoängAmerikanskaDelad > 2.5)
                {
                    Console.WriteLine("Ditt GPA indikerar på att dina betyg är i mitten. Med ditt GPA kan du komma in på de flesta universitet/högskolor i Amerika.");
                }
                Console.WriteLine("Om du planerar på att söka till en skola i ett annat land rekommenderas att du kontaktar skolan för mer info gällande betyg.");

                Console.WriteLine("");


                Console.WriteLine("Skapar ny csv-fil...");

                //Använder sig av "csv", "totalpoängAmerikanskDelad", "totalpoäng", "gymnasiebehörighet" och skickar dem till funktion "returneraFil"
                string länkTillTxt = ReturneraFil(csv, totalpoängAmerikanskaDelad, totalpoäng, gymnasiebehörighet, betyg);

                Console.WriteLine(".txt-fil genererad med länk: " + länkTillTxt);


            }
        }
        //Funktion som använder sig av string i Main och lägger in den som string "csv" i funktionen
        static double[] Omräknare(string csv)
        {
            //Använder sig av TextFieldParser och "csvParser" för att fortsätta med funktionen
            using (TextFieldParser csvParser = new TextFieldParser(csv))
            {
                //Använder csv för att skapa array
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.HasFieldsEnclosedInQuotes = true;

                //Läser nuvarande linje, går vidare till nästa linje
                csvParser.ReadLine();
                Console.WriteLine("Klar!");
                Console.WriteLine("");
                double[] meritvärden = new double[17];
                int x = 0;
                string[] fält;

                while (!csvParser.EndOfData)
                {
                    double värde;



                    // Read current line fields, pointer moves to the next line.
                    fält = csvParser.ReadFields();
                    string ämne = fält[0];
                    string betyg = fält[1];

                    //Switch för att läsa av och bestämma meritvärde på varje betyg
                    switch (betyg)
                    {
                        case "A":
                            värde = 20.0;
                            break;
                        case "B":
                            värde = 17.5;
                            break;
                        case "C":
                            värde = 15.0;
                            break;
                        case "D":
                            värde = 12.5;
                            break;
                        case "E":
                            värde = 10.0;
                            break;

                        case "F":
                            värde = 0.0;
                            break;

                        default:
                            värde = 0.0;
                            break;
                    }

                    meritvärden[x] = värde;
                    Console.WriteLine(ämne + " = " + betyg);
                    Console.WriteLine("Värde: " + värde + " poäng.");
                    Console.WriteLine("");
                    x++;



                }
                Console.WriteLine("Meritvärden samlade. Räknar...");
                return meritvärden;
                //return new double[] {-1};



            }

        }
        //Definerar funktion som länkar csv med string som är vald i main
        static int[] BetygSvenskaEngelska(string csv)
        {
            //Använder sig av TextFieldParser och "csvParser" för att fortsätta med funktionen
            using (TextFieldParser csvParser = new TextFieldParser(csv))
            {
                string[] värdenAttFölja = {"CLASS", "Bild", "Engelska", "Hemkunskap", "Idrott", "Matematik", "Musik", "Biologi", "Fysik", "Kemi", "Geografi", "Historia", "Religionskunskap", "Samhällskunskap", "Slöjd", "Svenska", "Teknik", "Språk" };

                //Skapar en ny array klasser som används innuti while-loop eftersom att annars så deklareras inte variabeln rätt.
                string[] klasser = new string[18];
                //Gör om fält i csv till array
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.HasFieldsEnclosedInQuotes = true;
                string ämnen;
                
                
              // csvParser.ReadLine();
                Console.WriteLine("Klar!");
                Console.WriteLine("");

                int x = 0;
                
                //Medans csvparser INTE är klar..
                while (!csvParser.EndOfData)
                {




                    //Läser nuvarande linje, går vidare till nästa linje
                    string[] fält = csvParser.ReadFields();
                    
                    string ämne = fält[0];
                    string betyg = fält[1];
                    ämnen = fält[0];
                    klasser[x] = ämne;
                    x++;







                }
                //Kollar så att engelska och svenska ligger på rätt plats, returnerar true eller false därav
                /* if (klasser[2] == värdenAttFölja[2] && klasser[15] == värdenAttFölja[15])
                {
                    return true;
                } else
                {
                    return false;
                } */
                int y = 0;
                int z = 0;
                int[] positionSvenskaEngelska = new int[2];
                
                while (y < klasser.Length)
                {
                    //Markerar på vilken plats i array som engelska ligger i
                    if (klasser[y] == "Engelska")
                    {
                        positionSvenskaEngelska[0] = y;
                    } 

                    y++;
                }
                while (z < klasser.Length)
                {
                    //Markerar på vilken plats i array som svenska ligger i.
                    if (klasser[z] == "Svenska")
                    {
                        positionSvenskaEngelska[1] = z;
                    } 

                    z++;
                }
                //Returnerar plats i array

                return positionSvenskaEngelska;

                

               











            }

        




        }
        //Funktion som använder sig av filer som defineras nedan
        static string ReturneraFil(string csv, double totalpoängAmerikanskaDelad, double totalpoäng, bool gymnasiebehörighet, double[] betyg)
        {
            //Skapar en array värdenAttFölja för att använda i while-loop.
            string[] värdenAttFölja = { "Bild", "Engelska", "Hemkunskap", "Idrott", "Matematik", "Musik", "Biologi", "Fysik", "Kemi", "Geografi", "Historia", "Religionskunskap", "Samhällskunskap", "Slöjd", "Svenska", "Teknik", "Språk" };
            int x = 0;
            int y = 0;
            string[] bokstav = new string[17];
            //Skapar en fil att skriva till
            string länkTillTxt = ("/Users/scandinaviana/Desktop/" + "genereradCSV" + ".csv");
            //Använder StreamWriter, prefix "sw" används för att bestämma vad som ska skrivas in i textfil.

            while (y < 17)
            {
                //Switch case som räknar om merit till betyg.
                switch (betyg[y])
                {
                    case 20.0:
                        bokstav[y] = "A";
                        break;
                    case 17.5:
                        bokstav[y] = "B";
                        break;
                    case 15.0:
                        bokstav[y] = "C";
                        break;
                    case 12.5:
                        bokstav[y] = "D";
                        break;
                    case 10.0:
                        bokstav[y] = "E";
                        break;

                    case 0.0:
                        bokstav[y] = "F";
                        break;

                    default:
                        bokstav[y] = "F";
                        break;
                }
                y++;

            }



            using (StreamWriter sw = File.CreateText(länkTillTxt))
                {
                sw.WriteLine("ÄMNE,BETYG,MERIT");
                    
                while( x < 17 )
                {
                    //Skriver ut varje ämne för sig, följt av betyg och meritvärde
                    sw.WriteLine(värdenAttFölja[x] + "," + bokstav[x] + "," + betyg[x] + "," + "," + "," + ",");
                    x++;
                }

                sw.WriteLine("-----", "-----");
                
                //Delar av i csv-filen för struktur
               

                //Skriver ut data som räknades i programmet
                sw.WriteLine("TOTAL MERIT," + totalpoäng);
                
                    sw.WriteLine("GPA," + totalpoängAmerikanskaDelad);
                if (gymnasiebehörighet)
                {
                    sw.WriteLine("GYMNASIEBEHÖRIGHET,JA");
                } else
                {
                    sw.WriteLine("GYMNASIEBEHÖRIGHET,JA");
                }
                //Skriver ut vilket datum filen skapades och med vilken csv-fil som programmet använde sig av
                sw.WriteLine("GENERERAD," + DateTime.Now);
                sw.WriteLine("ORIGINALFIL," + csv);

            }

            return länkTillTxt; //Returnerar länken till txt-filen som genereras.
            


        }



    }
}

    





