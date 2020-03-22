using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using System.Threading;
using System.Threading.Tasks;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace hazi2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        List<poz> input = new List<poz>();
        Rectangle porszivo = new Rectangle();
        Rectangle jart = new Rectangle();
        List<Rectangle> jart_teglalapok = new List<Rectangle>();
        const int SOR = 12;
        const int OSZLOP = 12;
        const int H = 50;
        const int W = 50;
        int j = 0;
        int szobaterulet = 1;
        static string[,] palya = new string[SOR, OSZLOP];
        const int faldb = 2 * SOR + 2 * OSZLOP + 2 * (SOR - 4) + 2 * (OSZLOP - 4);
        public enum irany
        {
            fel,
            le,
            jobbra,
            balra
        }
        public struct poz
        {
            public int x;
            public int y;
            public poz(int x, int y) : this()
            {
                this.x = x;
                this.y = y;
            }
        }
        VacCleaner VAC = new VacCleaner(palya, robotpoz, irany.fel);


        static poz robotpoz;
        List<poz> jartpoz = new List<poz>();  // dinamikus tárolóelem a bejárt területek tárolására
        poz[] falakpoz = new poz[100];
        public MainPage()
        {
            this.InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e) //pálya inic gomb event handler -> tömbbe
        {
            int x = 200;
            int y = 50;
            int k = 0;
            StreamReader reader = new StreamReader("robot.txt");
            for (int i = 0; i < SOR; i++)
            {
                string sor = reader.ReadLine();
                for (int j = 0; j < OSZLOP; j++)
                {
                    palya[i, j] = sor[j].ToString();
                }
            }
            for (int i = 0; i < SOR; i++)
            {
                for (int j = 0; j < OSZLOP; j++)
                {
                    if (alakzatvizsgal(palya[i, j]) == 0)
                    {
                        falrajzol(palya, x, y); //x=200tol 1450ig, y=50 tol 700 ig
                        falakpoz[k].x = x;
                        falakpoz[k].y = y;
                        k++;                      
                    }
                    else if (alakzatvizsgal(palya[i, j]) == 1)
                    {

                        robotrajzol(x, y);
                        robotpoz.x = x;
                        robotpoz.y = y;
                        VAC.setAktpoz(robotpoz);
                    }
                    else if (alakzatvizsgal(palya[i,j]) == 2)
                    {
                        szobaterulet++;
                    }
                    x += W;
                }
                if (x == 800)
                    x = 200;
                y += H;
            }
        }
        /* public int utkozes()
         {
             int x = 0;
             for (int i = 0; i < faldb; i++)
             {
                 if (Math.Abs(falakpoz[i].x - VAC.getAktpoz().x) < W && Math.Abs(falakpoz[i].y - VAC.getAktpoz().y) < H)
                 {
                     porszivo.Fill = new SolidColorBrush(Windows.UI.Colors.Green);
                     x = 1;
                     break;
                 }
                 else
                     x = 0;
             }
             return x;
         }*/
        public int VACutkozes()
        {
            int oszlop = 0;
            int sor = 0;
            int x = 0;
            List<poz> sens = new List<poz>();
            switch (VAC.getIrany())
            {
                case irany.fel:
                    {
                        int k = 0;
                        int l = 0;
                        oszlop = (VAC.getAktpoz().x - 200) / 50 - 2;
                        sor = (VAC.getAktpoz().y - 50) / 50 - 1;
                        for (int i = sor; i > sor - 2; i--)
                        {
                            for (int j = oszlop; j < oszlop + 5; j++)
                            {
                                if (alakzatvizsgal(VAC.getSensor()[k, l]) == 0)
                                {
                                    sens.Add(new poz((j * W) + 200, (i * H) + 50));
                                }
                                l++;
                            }
                            l = 0;
                            k++;
                        }
                    }
                    break;
                case irany.le:
                    {
                        int k = 0;
                        int l = 0;
                        oszlop = (VAC.getAktpoz().x - 200) / 50 + 2;
                        sor = (VAC.getAktpoz().y - 50) / 50 + 1;
                        for (int i = sor; i < sor + 2; i++)
                        {
                            for (int j = oszlop; j > oszlop - 5; j--)
                            {

                                if (alakzatvizsgal(VAC.getSensor()[k, l]) == 0)
                                {
                                    sens.Add(new poz((j * W) + 200, (i * H) + 50));
                                }
                                l++;
                            }
                            l = 0;
                            k++;
                        }
                    }
                    break;
                case irany.jobbra:
                    {
                        int k = 0;
                        int l = 0;
                        oszlop = (VAC.getAktpoz().x - 200) / 50 + 1;
                        sor = (VAC.getAktpoz().y - 50) / 50 - 2;
                        for (int j = oszlop; j < oszlop + 2; j++)
                        {
                            for (int i = sor; i < sor + 5; i++)
                            {

                                if (alakzatvizsgal(VAC.getSensor()[k, l]) == 0)
                                {
                                    sens.Add(new poz((j * W) + 200, (i * H) + 50));
                                }
                                l++;
                            }
                            l = 0;
                            k++;
                        }
                    }
                    break;
                case irany.balra:
                    {
                        int k = 0;
                        int l = 0;
                        oszlop = (VAC.getAktpoz().x - 200) / 50 - 1;
                        sor = (VAC.getAktpoz().y - 50) / 50 + 2;
                        for (int j = oszlop; j > oszlop - 2; j--)
                        {
                            for (int i = sor; i > sor - 5; i--)
                            {

                                if (alakzatvizsgal(VAC.getSensor()[k, l]) == 0)
                                {
                                    sens.Add(new poz((j * W) + 200, (i * H) + 50));
                                }
                                l++;
                            }
                            l = 0;
                            k++;
                        }
                    }
                    break;
            }
            for (int i = 0; i < sens.Count; i++)
            {
                if (Math.Abs(VAC.getAktpoz().x - sens[i].x) < 2 * W && Math.Abs(VAC.getAktpoz().y - sens[i].y) < 2 * H)
                {
                    switch (VAC.getIrany())
                    {
                        case irany.fel:
                            {
                                if (sens[i].x == VAC.getAktpoz().x)
                                {
                                    x = 1;
                                   // porszivo.Fill = new SolidColorBrush(Windows.UI.Colors.Green);
                                }
                            }
                            break;
                        case irany.le:
                            {
                                if (sens[i].x == VAC.getAktpoz().x)
                                {
                                    x = 1;
                                    //porszivo.Fill = new SolidColorBrush(Windows.UI.Colors.Green);
                                }
                            }
                            break;
                        case irany.jobbra:
                            {
                                if (sens[i].y == VAC.getAktpoz().y)
                                {
                                    x = 1;
                                   // porszivo.Fill = new SolidColorBrush(Windows.UI.Colors.Green);
                                }
                            }
                            break;
                        case irany.balra:
                            {
                                if (sens[i].y == VAC.getAktpoz().y)
                                {
                                    x = 1;
                                    //porszivo.Fill = new SolidColorBrush(Windows.UI.Colors.Green);
                                }
                            }
                            break;
                    }
                }
                else
                {
                    if (x != 1)
                    {
                        x = 0;
                    }
                }
            }
            return x;
        }
        public void buttonelore_Click(object sender, RoutedEventArgs e)
        {
            robotfel(VAC.getAktpoz());
        }
        private void buttonbalra1_Click(object sender, RoutedEventArgs e)
        {
            robotbal(VAC.getAktpoz());
        }
        private void buttonjobbra_Click(object sender, RoutedEventArgs e)
        {
            robotjobb(VAC.getAktpoz());
        }
        private void buttonhatra_Click(object sender, RoutedEventArgs e)
        {
            robotle(VAC.getAktpoz());
        }

        int progressbarint = 0;
        private void progressbar()
        {
            if (!jartpoz.Contains(VAC.getAktpoz()))
            {
                progressbarint++;
                lefedettség.Value = progressbarint;
            }
        }

        private void jart_kirajzol()
        {
            for (int i = j; i < jart_teglalapok.Count; i++)
            {
                jart_teglalapok[i].Fill = new SolidColorBrush(Windows.UI.Colors.LightGray);
                jart_teglalapok[i].Width = W;
                jart_teglalapok[i].Height = H;
                jart_teglalapok[i].Stroke = new SolidColorBrush(Windows.UI.Colors.Black);
                jart_teglalapok[i].StrokeThickness = 1;

                Canvas.Children.Add(jart_teglalapok[i]);

                Canvas.SetLeft(jart_teglalapok[i], jartpoz[i].x);
                Canvas.SetTop(jart_teglalapok[i], jartpoz[i].y);
                j++;
            }
        }

        // 4 irányba történő mozgatás, a bejárt területek folyamatos lementése, a progressbar növelése új terület érintésekor
        public void robotfel(poz p)
        {
            VAC.getData();
            VAC.iranyvalt(irany.fel);
            Canvas.SetLeft(porszivo, p.x);
            Canvas.SetTop(porszivo, p.y - H);
            jartpoz.Add(new poz(VAC.getAktpoz().x, VAC.getAktpoz().y));
            jart_teglalapok.Add(new Rectangle());
            jart_kirajzol();
            VAC.leptet();

            progressbar();
            //utkozes();
        }
        public void robotle(poz p)
        {
            VAC.getData();
            VAC.iranyvalt(irany.le);
            Canvas.SetLeft(porszivo, p.x);
            Canvas.SetTop(porszivo, p.y + H);
            jartpoz.Add(new poz(VAC.getAktpoz().x, VAC.getAktpoz().y));
            jart_teglalapok.Add(new Rectangle());
            jart_kirajzol();
            VAC.leptet();

            progressbar();
            //utkozes();
        }
        public void robotjobb(poz p)
        {
            VAC.getData();
            VAC.iranyvalt(irany.jobbra);
            Canvas.SetLeft(porszivo, p.x + W);
            Canvas.SetTop(porszivo, p.y);
            jartpoz.Add(new poz(VAC.getAktpoz().x, VAC.getAktpoz().y));
            jart_teglalapok.Add(new Rectangle());
            jart_kirajzol();
            VAC.leptet();

            progressbar();
            // utkozes();
        }
        public void robotbal(poz p)
        {
            VAC.getData();
            VAC.iranyvalt(irany.balra);
            Canvas.SetLeft(porszivo, p.x - W);
            Canvas.SetTop(porszivo, p.y);
            jartpoz.Add(new poz(VAC.getAktpoz().x, VAC.getAktpoz().y));
            jart_teglalapok.Add(new Rectangle());
            jart_kirajzol();
            VAC.leptet();

            progressbar();
            //utkozes();
        }

        //falak kirajzolása -> kék négyzet
        private void falrajzol(string[,] tomb, int x, int y)
        {
            Rectangle fal = new Rectangle();
            fal.Fill = new SolidColorBrush(Windows.UI.Colors.Blue);
            fal.Width = W;
            fal.Height = H;
            fal.Stroke = new SolidColorBrush(Windows.UI.Colors.Black);
            fal.StrokeThickness = 1;

            Canvas.Children.Add(fal);

            Canvas.SetLeft(fal, x);
            Canvas.SetTop(fal, y);
        }

        //robot kirajzolása -> piros négyzet
        private void robotrajzol(int x, int y)
        {
            porszivo.Fill = new SolidColorBrush(Windows.UI.Colors.Red);
            porszivo.Width = W;
            porszivo.Height = H;
            porszivo.Stroke = new SolidColorBrush(Windows.UI.Colors.Black);
            porszivo.StrokeThickness = 1;

            Canvas.Children.Add(porszivo);

            Canvas.SetLeft(porszivo, x);
            Canvas.SetTop(porszivo, y);
            Canvas.SetZIndex(porszivo, 1);
        }

        //alakzatvizsgálás a txt-hez -> o: fal, x: robot
        private int alakzatvizsgal(string s)
        {
            if (s == "o")
                return 0;
            else if (s == "x")
                return 1;
            else if (s == ".")
                return 2;
            else
                return 3;
        }

        private void lefedettség_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            lefedettség.Maximum = szobaterulet;
            lefedettség.Value = progressbarint;
        }

        public void VACrajzol()
        {
            Canvas.SetLeft(porszivo, (VAC.getAktpoz().x));
            Canvas.SetTop(porszivo, (VAC.getAktpoz().y));
            jartpoz.Add(new poz(VAC.getAktpoz().x, VAC.getAktpoz().y));
            jart_teglalapok.Add(new Rectangle());
            jart_kirajzol();
        }


        public async void algoritmus1()
        {
            int milisec = 500;
            while (true)
            {
                VAC.getData();
                if (VACutkozes() == 1)
                {
                    VAC.jobbra();
                    VAC.getData();
                }
                VACrajzol();
                VAC.leptet();
                progressbar();
                await Task.Delay(milisec);
            }
        }

        private void alg1_Click(object sender, RoutedEventArgs e)
        {
            algoritmus1();
        }

    }

}


