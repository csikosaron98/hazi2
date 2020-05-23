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
using Microsoft.VisualBasic;
using Windows.UI.Xaml.Documents;
using Windows.Media.Audio;
using System.Globalization;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Store.Preview.InstallControl;
using Windows.Media.Playback;
using Windows.Media.Core;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace hazi2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //VÁLTOZÓK

        List<poz> falakpoz = new List<poz>();
        List<Rectangle> jart_teglalapok = new List<Rectangle>();
        Rectangle porszivo = new Rectangle();
        Rectangle jart = new Rectangle();
        static poz robotpoz;
        VacCleaner VAC = new VacCleaner(palya, robotpoz, irany.fel);
        public const int SOR = 20;
        public const int OSZLOP = 20;
        public const int H = 33;
        public const int W = 33;
        int j = 0;
        int progressbarint = 0;
        int szobaterulet = 1;
        bool algdone = false;
        string room = "";
        static string[,] palya = new string[SOR, OSZLOP];

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
        public MainPage()
        {
            this.InitializeComponent();
        }

        //FÜGGVÉNYEK

        //alakzatvizsgálás a txt-hez -> o: fal, x: robot, .: semmi
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
        //függvény a robot esetleges ötközésének vizsgálatára -> visszatérés: 0 (ütközik), 1 (nem ütközik)
        public int VACutkozes()
        {
            int oszlop = 0;
            int sor = 0;
            int x = 2;
            List<poz> sens = new List<poz>();
            switch (VAC.getIrany())
            {
                case irany.fel:
                    {
                        int k = 0;
                        int l = 0;
                        oszlop = (VAC.getAktpoz().x - 4 * H) / H - 2;
                        sor = (VAC.getAktpoz().y - H) / H - 1;
                        for (int i = sor; i > sor - 2; i--)
                        {
                            for (int j = oszlop; j < oszlop + 5; j++)
                            {
                                if (alakzatvizsgal(VAC.getSensor()[k, l]) == 0)
                                {
                                    sens.Add(new poz((j * W) + 4 * H, (i * H) + H));
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
                        oszlop = (VAC.getAktpoz().x - 4 * H) / H + 2;
                        sor = (VAC.getAktpoz().y - H) / H + 1;
                        for (int i = sor; i < sor + 2; i++)
                        {
                            for (int j = oszlop; j > oszlop - 5; j--)
                            {

                                if (alakzatvizsgal(VAC.getSensor()[k, l]) == 0)
                                {
                                    sens.Add(new poz((j * W) + 4 * H, (i * H) + H));
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
                        oszlop = (VAC.getAktpoz().x - 4 * H) / H + 1;
                        sor = (VAC.getAktpoz().y - H) / H - 2;
                        for (int j = oszlop; j < oszlop + 2; j++)
                        {
                            for (int i = sor; i < sor + 5; i++)
                            {

                                if (alakzatvizsgal(VAC.getSensor()[k, l]) == 0)
                                {
                                    sens.Add(new poz((j * W) + 4 * H, (i * H) + H));
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
                        oszlop = (VAC.getAktpoz().x - 4 * H) / H - 1;
                        sor = (VAC.getAktpoz().y - H) / H + 2;
                        for (int j = oszlop; j > oszlop - 2; j--)
                        {
                            for (int i = sor; i > sor - 5; i--)
                            {

                                if (alakzatvizsgal(VAC.getSensor()[k, l]) == 0)
                                {
                                    sens.Add(new poz((j * W) + 4 * H, (i * H) + H));
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
                                    x = 0;
                                }
                            }
                            break;
                        case irany.le:
                            {
                                if (sens[i].x == VAC.getAktpoz().x)
                                {
                                    x = 0;
                                }
                            }
                            break;
                        case irany.jobbra:
                            {
                                if (sens[i].y == VAC.getAktpoz().y)
                                {
                                    x = 0;
                                }
                            }
                            break;
                        case irany.balra:
                            {
                                if (sens[i].y == VAC.getAktpoz().y)
                                {
                                    x = 0;
                                }
                            }
                            break;
                    }
                }
                else
                {
                    if (x != 0)
                    {
                        x = 1;
                    }
                }
            }
            return x;
        }
        public int VACjart()
        {
            int x = 2;
            switch (VAC.getIrany())
            {
                case irany.fel:
                    {
                        poz tmp = VAC.getAktpoz();
                        tmp.y = tmp.y - H;
                        if (VAC.getJartrect().Contains(tmp))
                        {
                            x = 0;
                            break;
                        }
                        else
                            x = 1;
                    }
                    break;
                case irany.le:
                    {
                        poz tmp = VAC.getAktpoz();
                        tmp.y = tmp.y + H;
                        if (VAC.getJartrect().Contains(tmp))
                        {
                            x = 0;
                            break;
                        }
                        else
                            x = 1;
                    }
                    break;
                case irany.jobbra:
                    {
                        poz tmp = VAC.getAktpoz();
                        tmp.x = tmp.x + W;
                        if (VAC.getJartrect().Contains(tmp))
                        {
                            x = 0;
                            break;
                        }
                        else
                            x = 1;
                    }
                    break;
                case irany.balra:
                    {
                        poz tmp = VAC.getAktpoz();
                        tmp.x = tmp.x - W;
                        if (VAC.getJartrect().Contains(tmp))
                        {
                            x = 0;
                            break;
                        }
                        else
                            x = 1;
                    }
                    break;
            }
            return x;
        }
        private void progressbar() 
        {
            if (!VAC.getJartrect().Contains(VAC.getAktpoz()))
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

                Canvas.SetLeft(jart_teglalapok[i], VAC.getJartrect()[i].x);
                Canvas.SetTop(jart_teglalapok[i], VAC.getJartrect()[i].y);
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
            VAC.getJartrect().Add(new poz(VAC.getAktpoz().x, VAC.getAktpoz().y));
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
            VAC.getJartrect().Add(new poz(VAC.getAktpoz().x, VAC.getAktpoz().y));
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
            VAC.getJartrect().Add(new poz(VAC.getAktpoz().x, VAC.getAktpoz().y));
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
            VAC.getJartrect().Add(new poz(VAC.getAktpoz().x, VAC.getAktpoz().y));
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
        public void VACrajzol()
        {
            progressbar();
            Canvas.SetLeft(porszivo, (VAC.getAktpoz().x));
            Canvas.SetTop(porszivo, (VAC.getAktpoz().y));
            VAC.getJartrect().Add(new poz(VAC.getAktpoz().x, VAC.getAktpoz().y));
            jart_teglalapok.Add(new Rectangle());
            jart_kirajzol();
        }

        //ALGORITMUSOK

        //algoritmus-változók
        int move_milisec = 200;     //a robot sebességéért felelős változó
        int think_milisec = 1000;   //a robot algoritmusok közti "gondolkodásáért" felelős változó
        int which_leftright = 0;      //változó az "okos" snake algoritmushoz
        int sign_nochance = 0;       //változó az algoritmusok egyes esélyeinek/életeinek lejárásának jelzésére
        poz check;      //változó az algoritmus közbeni üzközés esetén az adott pozíció lementésére
        int counter_firstcheck = 0;     //számláló-változó az esetleges algoritmus közbeni ütközések számlálásához
        int counter_washere;        //számláló-változó az esetleges algoritmus közbeni ütközések számlálásához

        //algoritmus-függvények
        public async Task alg_random_egyszererint()
        {
            while (true)
            {
                int alert = 1;
                Random r = new Random();
                int genRand = r.Next(1, 10);
                VAC.getData();
                VACrajzol();
                if (VACutkozes() == 0)
                {
                    alert = 0;
                }
                if (VACjart() == 0)
                {
                    alert = 0;
                }
                if (alert == 0)
                {
                    if (genRand <= 5)
                    {
                        VAC.jobbra();
                    }
                    else if (genRand > 5)
                    {
                        VAC.balra();
                    }
                    VAC.getData();
                }
                else
                {
                    VAC.leptet();
                    VACrajzol();
                }
                await Task.Delay(move_milisec);
            }
        }
        public async Task circle()
        {
            int count_turn = 1;
            double count_steps = 1;
            int chance = 10;
            VAC.getData();
            while (VACutkozes() != 0)
            {
                for (int i = 0; i < count_steps; i++)
                {
                    VACrajzol();
                    VAC.getData();
                    if (VACutkozes() == 0)
                    {
                        porszivo.Fill = new SolidColorBrush(Windows.UI.Colors.Yellow);
                        break;
                    }
                    VAC.leptet();
                    VACrajzol();
                    await Task.Delay(move_milisec);
                    if (i % count_turn == 0)
                    {
                        VAC.balra();
                    }
                    if (VAC.getJartrect().Contains(VAC.GetFrontPoz())) //ha oda lépek ahol már jártam, egy esély-
                    {
                        chance--;
                        if (chance == 0)
                            return;
                    }
                }
                count_turn++;
                count_steps = count_steps + 0.5;
            }
        }
        public async Task wallfollow_left_escape()
        {
            poz now;
            now = VAC.getAktpoz();
            VAC.getData();
            VACrajzol();
            while (VACutkozes() != 0) //ütközésig megy
            {
                VAC.getData();
                VAC.leptet();
                VACrajzol();
                await Task.Delay(move_milisec);
                VAC.getData();
            }
            irany tmp = VAC.getIrany();
            bool quickleft = false;
            VAC.jobbra();
            VAC.getData();
            if (VAC.getSensor()[0, 1] == ".")
            {
                quickleft = true;
            }
            VAC.leptet();
            VACrajzol();
            await Task.Delay(move_milisec);
            VAC.getData();
            if (quickleft)
            {
                VAC.balra();
                VAC.getData();
            }
            if (VACutkozes() == 0)
            {
                if (counter_firstcheck == 0)
                {
                    check = VAC.getAktpoz();
                    counter_firstcheck++;
                }
            }
            if (tmp == irany.balra)
            {
                while (VAC.getAktpoz().y != now.y)
                {
                    if (check.x == VAC.getAktpoz().x && check.y == VAC.getAktpoz().y)
                        counter_washere++;
                    if (VAC.getAktpoz().y == now.y)
                        return;
                    if (alakzatvizsgal(VAC.getSensor()[0, 1]) == 0 && VACutkozes() != 0) //fal mellett való léptetés
                    {
                        VAC.getData();
                        VAC.leptet();
                        VACrajzol();
                        VAC.getData();
                        await Task.Delay(move_milisec);
                    }
                    else if (VACutkozes() == 0)
                    {
                        bool justone = false;
                        VAC.getData();
                        if (VAC.getSensor()[0, 3] == ".")
                        {
                            justone = true;
                        }
                        VAC.jobbra();
                        VAC.getData();
                        if (VACutkozes() == 0)
                            continue;
                        VAC.leptet();
                        VACrajzol();
                        VAC.getData();
                        await Task.Delay(move_milisec);
                        if (justone)
                        {
                            VAC.balra();
                            VAC.getData();
                        }
                    }
                    else if (alakzatvizsgal(VAC.getSensor()[0, 1]) != 0 && VACutkozes() != 0)
                    {
                        VAC.getData();
                        VAC.leptet();
                        VACrajzol();
                        VAC.getData();
                        await Task.Delay(move_milisec);
                        VAC.balra();
                        VAC.getData();
                        if (VACutkozes() == 0)
                            continue;
                        VAC.leptet();
                        VACrajzol();
                        VAC.getData();
                        await Task.Delay(move_milisec);
                    }
                }
            }
            if (tmp == irany.fel)
            {
                while (VAC.getAktpoz().x != now.x)
                {
                    if (check.x == VAC.getAktpoz().x && check.y == VAC.getAktpoz().y)
                        counter_washere++;
                    if (VAC.getAktpoz().x == now.x)
                        return;
                    if (alakzatvizsgal(VAC.getSensor()[0, 1]) == 0 && VACutkozes() != 0) //fal mellett való léptetés
                    {
                        VAC.getData();
                        VAC.leptet();
                        VACrajzol();
                        VAC.getData();
                        await Task.Delay(move_milisec);
                    }
                    else if (VACutkozes() == 0)
                    {
                        bool justone = false;
                        VAC.getData();
                        if (VAC.getSensor()[0, 3] == ".")
                        {
                            justone = true;
                        }
                        VAC.jobbra();
                        VAC.getData();
                        if (VACutkozes() == 0)
                            continue;
                        VAC.leptet();
                        VACrajzol();
                        VAC.getData();
                        await Task.Delay(move_milisec);
                        if (justone)
                        {
                            VAC.balra();
                            VAC.getData();
                        }
                    }
                    else if (alakzatvizsgal(VAC.getSensor()[0, 1]) != 0 && VACutkozes() != 0)
                    {
                        VAC.getData();
                        VAC.leptet();
                        VACrajzol();
                        VAC.getData();
                        await Task.Delay(move_milisec);
                        VAC.balra();
                        VAC.getData();
                        if (VACutkozes() == 0)
                            continue;
                        VAC.leptet();
                        VACrajzol();
                        VAC.getData();
                        await Task.Delay(move_milisec);
                    }
                }
            }
            if (tmp == irany.le)
            {
                while (VAC.getAktpoz().x != now.x)
                {
                    if (check.x == VAC.getAktpoz().x && check.y == VAC.getAktpoz().y)
                        counter_washere++;
                    if (VAC.getAktpoz().x == now.x)
                        return;
                    if (alakzatvizsgal(VAC.getSensor()[0, 1]) == 0 && VACutkozes() != 0) //fal mellett való léptetés
                    {
                        VAC.getData();
                        VAC.leptet();
                        VACrajzol();
                        VAC.getData();
                        await Task.Delay(move_milisec);
                    }
                    else if (VACutkozes() == 0)
                    {
                        bool justone = false;
                        VAC.getData();
                        if (VAC.getSensor()[0, 3] == ".")
                        {
                            justone = true;
                        }
                        VAC.jobbra();
                        VAC.getData();
                        if (VACutkozes() == 0)
                            continue;
                        VAC.leptet();
                        VACrajzol();
                        VAC.getData();
                        await Task.Delay(move_milisec);
                        if (justone)
                        {
                            VAC.balra();
                            VAC.getData();
                        }
                    }
                    else if (alakzatvizsgal(VAC.getSensor()[0, 1]) != 0 && VACutkozes() != 0)
                    {
                        VAC.getData();
                        VAC.leptet();
                        VACrajzol();
                        VAC.getData();
                        await Task.Delay(move_milisec);
                        VAC.balra();
                        VAC.getData();
                        if (VACutkozes() == 0)
                            continue;
                        VAC.leptet();
                        VACrajzol();
                        VAC.getData();
                        await Task.Delay(move_milisec);
                    }
                }
            }
            if (tmp == irany.jobbra)
            {
                while (VAC.getAktpoz().y != now.y)
                {
                    if (check.x == VAC.getAktpoz().x && check.y == VAC.getAktpoz().y)
                        counter_washere++;
                    if (VAC.getAktpoz().y == now.y)
                        return;
                    if (alakzatvizsgal(VAC.getSensor()[0, 1]) == 0 && VACutkozes() != 0) //fal mellett való léptetés
                    {
                        VAC.getData();
                        VAC.leptet();
                        VACrajzol();
                        VAC.getData();
                        await Task.Delay(move_milisec);
                    }
                    else if (VACutkozes() == 0)
                    {
                        bool justone = false;
                        VAC.getData();
                        if (VAC.getSensor()[0, 3] == ".")
                        {
                            justone = true;
                        }
                        VAC.jobbra();
                        VAC.getData();
                        if (VACutkozes() == 0)
                            continue;
                        VAC.leptet();
                        VACrajzol();
                        VAC.getData();
                        await Task.Delay(move_milisec);
                        if (justone)
                        {
                            VAC.balra();
                            VAC.getData();
                        }
                    }
                    else if (alakzatvizsgal(VAC.getSensor()[0, 1]) != 0 && VACutkozes() != 0)
                    {
                        VAC.getData();
                        VAC.leptet();
                        VACrajzol();
                        VAC.getData();
                        await Task.Delay(move_milisec);
                        VAC.balra();
                        VAC.getData();
                        if (VACutkozes() == 0)
                            continue;
                        VAC.leptet();
                        VACrajzol();
                        VAC.getData();
                        await Task.Delay(move_milisec);
                    }
                }
            }
        }
        public async Task wallfollow_right_escape()
        {
            poz now;
            now = VAC.getAktpoz();
            VAC.getData();
            VACrajzol();
            while (VACutkozes() != 0) //ütközésig megy
            {
                VAC.getData();
                VAC.leptet();
                VACrajzol();
                await Task.Delay(move_milisec);
                VAC.getData();
            }
            irany tmp = VAC.getIrany();
            bool quickright = false;
            VAC.balra();
            VAC.getData();
            if (VAC.getSensor()[0, 3] == ".")
            {
                quickright = true;
            }
            VAC.leptet();
            VACrajzol();
            await Task.Delay(move_milisec);
            VAC.getData();
            if (quickright)
            {
                VAC.jobbra();
                VAC.getData();
            }
            if (VACutkozes() == 0)
            {
                if (counter_firstcheck == 0)
                {
                    check = VAC.getAktpoz();
                    counter_firstcheck++;
                }
            }
            if (tmp == irany.jobbra)
            {
                while (VAC.getAktpoz().y != now.y)
                {
                    if (check.x == VAC.getAktpoz().x && check.y == VAC.getAktpoz().y)
                        counter_washere++;
                    if (VAC.getAktpoz().y == now.y)
                        return;
                    if (alakzatvizsgal(VAC.getSensor()[0, 3]) == 0 && VACutkozes() != 0) //fal mellett való léptetés
                    {
                        VAC.getData();
                        VAC.leptet();
                        VACrajzol();
                        VAC.getData();
                        await Task.Delay(move_milisec);
                    }
                    else if (VACutkozes() == 0)
                    {
                        bool justone = false;
                        VAC.getData();
                        if (VAC.getSensor()[0, 1] == ".")
                        {
                            justone = true;
                        }
                        VAC.balra();
                        VAC.getData();
                        if (VACutkozes() == 0)
                            continue;
                        VAC.leptet();
                        VACrajzol();
                        VAC.getData();
                        await Task.Delay(move_milisec);
                        if (justone)
                        {
                            VAC.jobbra();
                            VAC.getData();
                        }
                    }
                    else if (alakzatvizsgal(VAC.getSensor()[0, 3]) != 0 && VACutkozes() != 0)
                    {
                        VAC.getData();
                        VAC.leptet();
                        VACrajzol();
                        VAC.getData();
                        await Task.Delay(move_milisec);
                        VAC.jobbra();
                        VAC.getData();
                        if (VACutkozes() == 0)
                            continue;
                        VAC.leptet();
                        VACrajzol();
                        VAC.getData();
                        await Task.Delay(move_milisec);
                    }
                }
            }
            if (tmp == irany.balra)
            {
                while (VAC.getAktpoz().y != now.y)
                {
                    if (check.x == VAC.getAktpoz().x && check.y == VAC.getAktpoz().y)
                        counter_washere++;
                    if (VAC.getAktpoz().y == now.y)
                        return;
                    if (alakzatvizsgal(VAC.getSensor()[0, 3]) == 0 && VACutkozes() != 0) //fal mellett való léptetés
                    {
                        VAC.getData();
                        VAC.leptet();
                        VACrajzol();
                        VAC.getData();
                        await Task.Delay(move_milisec);
                    }
                    else if (VACutkozes() == 0)
                    {
                        bool justone = false;
                        VAC.getData();
                        if (VAC.getSensor()[0, 1] == ".")
                        {
                            justone = true;
                        }
                        VAC.balra();
                        VAC.getData();
                        if (VACutkozes() == 0)
                            continue;
                        VAC.leptet();
                        VACrajzol();
                        VAC.getData();
                        await Task.Delay(move_milisec);
                        if (justone)
                        {
                            VAC.jobbra();
                            VAC.getData();
                        }
                    }
                    else if (alakzatvizsgal(VAC.getSensor()[0, 3]) != 0 && VACutkozes() != 0)
                    {
                        VAC.getData();
                        VAC.leptet();
                        VACrajzol();
                        VAC.getData();
                        await Task.Delay(move_milisec);
                        VAC.jobbra();
                        VAC.getData();
                        if (VACutkozes() == 0)
                            continue;
                        VAC.leptet();
                        VACrajzol();
                        VAC.getData();
                        await Task.Delay(move_milisec);
                    }
                }
            }
            if (tmp == irany.le)
            {
                while (VAC.getAktpoz().x != now.x)
                {
                    if (check.x == VAC.getAktpoz().x && check.y == VAC.getAktpoz().y)
                        counter_washere++;
                    if (VAC.getAktpoz().x == now.x)
                        return;
                    if (alakzatvizsgal(VAC.getSensor()[0, 3]) == 0 && VACutkozes() != 0) //fal mellett való léptetés
                    {
                        VAC.getData();
                        VAC.leptet();
                        VACrajzol();
                        VAC.getData();
                        await Task.Delay(move_milisec);
                    }
                    else if (VACutkozes() == 0)
                    {
                        bool justone = false;
                        VAC.getData();
                        if (VAC.getSensor()[0, 1] == ".")
                        {
                            justone = true;
                        }
                        VAC.balra();
                        VAC.getData();
                        if (VACutkozes() == 0)
                            continue;
                        VAC.leptet();
                        VACrajzol();
                        VAC.getData();
                        await Task.Delay(move_milisec);
                        if (justone)
                        {
                            VAC.jobbra();
                            VAC.getData();
                        }
                    }
                    else if (alakzatvizsgal(VAC.getSensor()[0, 3]) != 0 && VACutkozes() != 0)
                    {
                        VAC.getData();
                        VAC.leptet();
                        VACrajzol();
                        VAC.getData();
                        await Task.Delay(move_milisec);
                        VAC.jobbra();
                        VAC.getData();
                        if (VACutkozes() == 0)
                            continue;
                        VAC.leptet();
                        VACrajzol();
                        VAC.getData();
                        await Task.Delay(move_milisec);
                    }
                }
            }
            if (tmp == irany.fel)
            {
                while (VAC.getAktpoz().x != now.x)
                {
                    if (check.x == VAC.getAktpoz().x && check.y == VAC.getAktpoz().y)
                        counter_washere++;
                    if (VAC.getAktpoz().x == now.x)
                        return;
                    if (alakzatvizsgal(VAC.getSensor()[0, 3]) == 0 && VACutkozes() != 0) //fal mellett való léptetés
                    {
                        VAC.getData();
                        VAC.leptet();
                        VACrajzol();
                        VAC.getData();
                        await Task.Delay(move_milisec);
                    }
                    else if (VACutkozes() == 0)
                    {
                        bool justone = false;
                        VAC.getData();
                        if (VAC.getSensor()[0, 1] == ".")
                        {
                            justone = true;
                        }
                        VAC.balra();
                        VAC.getData();
                        if (VACutkozes() == 0)
                            continue;
                        VAC.leptet();
                        VACrajzol();
                        VAC.getData();
                        await Task.Delay(move_milisec);
                        if (justone)
                        {
                            VAC.jobbra();
                            VAC.getData();
                        }
                    }
                    else if (alakzatvizsgal(VAC.getSensor()[0, 3]) != 0 && VACutkozes() != 0)
                    {
                        VAC.getData();
                        VAC.leptet();
                        VACrajzol();
                        VAC.getData();
                        await Task.Delay(move_milisec);
                        VAC.jobbra();
                        VAC.getData();
                        if (VACutkozes() == 0)
                            continue;
                        VAC.leptet();
                        VACrajzol();
                        VAC.getData();
                        await Task.Delay(move_milisec);
                    }
                }
            }
        }
        public async Task wallfollow_left_goaround()
        {
            //int chance = 10;
            poz now;
            VAC.getData();
            VACrajzol();
            while (VACutkozes() != 0) //ütközésig megy
            {
                VAC.getData();
                VAC.leptet();
                VACrajzol();
                await Task.Delay(move_milisec);
                VAC.getData();
            }

            bool quickleft = false;
            now = VAC.getAktpoz();
            VAC.jobbra();
            VAC.getData();
            if (VAC.getSensor()[0, 1] == ".")
            {
                quickleft = true;
            }
            VAC.leptet();
            VACrajzol();
            await Task.Delay(move_milisec);
            VAC.getData();
            if (quickleft)
            {
                VAC.balra();
                VAC.getData();
            }
            poz tmp;
            int balravizsgal = 0; // változó -> jó irányba történő vizsgáláshoz

            while (VAC.getAktpoz().x != now.x || VAC.getAktpoz().y != now.y)
            {
                if (alakzatvizsgal(VAC.getSensor()[0, 1]) == 0 && VACutkozes() != 0)
                {
                    VAC.getData();
                    VAC.leptet();
                    VACrajzol();
                    VAC.getData();
                    await Task.Delay(move_milisec);
                }
                else if (VACutkozes() == 0)
                {
                    bool justone = false;
                    VAC.getData();
                    if (VAC.getSensor()[0, 3] == ".")
                    {
                        justone = true;
                    }
                    VAC.jobbra();
                    VAC.getData();
                    if (VACutkozes() == 0)
                        continue;
                    VAC.leptet();
                    VACrajzol();
                    VAC.getData();
                    await Task.Delay(move_milisec);
                    if (justone)
                    {
                        VAC.balra();
                        VAC.getData();
                    }
                }
                else if (alakzatvizsgal(VAC.getSensor()[0, 1]) != 0 && VACutkozes() != 0)
                {
                    balravizsgal = 1;
                    VAC.getData();
                    VAC.leptet();
                    VACrajzol();
                    VAC.getData();
                    await Task.Delay(move_milisec);
                    VAC.balra();
                    VAC.getData();
                    if (VACutkozes() == 0)
                        continue;
                    VAC.leptet();
                    VACrajzol();
                    VAC.getData();
                    await Task.Delay(move_milisec);
                }
                if (balravizsgal == 1) // nem volt szinkronban a tmp vizsgálat és a robot léptetés
                {
                    VAC.balra(); //irányba állítás
                    switch (VAC.getIrany()) //vizsgálat
                    {
                        case irany.fel:
                            tmp.x = VAC.getAktpoz().x;
                            tmp.y = VAC.getAktpoz().y - H;
                            break;
                        case irany.le:
                            tmp.x = VAC.getAktpoz().x;
                            tmp.y = VAC.getAktpoz().y + H;
                            break;
                        case irany.jobbra:
                            tmp.x = VAC.getAktpoz().x + H;
                            tmp.y = VAC.getAktpoz().y;
                            break;
                        case irany.balra:
                            tmp.x = VAC.getAktpoz().x - H;
                            tmp.y = VAC.getAktpoz().y;
                            break;
                        default:
                            tmp.x = 10000;
                            tmp.y = 10000;
                            break;
                    }
                    VAC.jobbra(); //eredeti-vissza irányba állítás
                    balravizsgal = 0;
                }
                else //ha szinkronban volt a két művelet
                {
                    switch (VAC.getIrany())
                    {
                        case irany.fel:
                            tmp.x = VAC.getAktpoz().x;
                            tmp.y = VAC.getAktpoz().y - H;
                            break;
                        case irany.le:
                            tmp.x = VAC.getAktpoz().x;
                            tmp.y = VAC.getAktpoz().y + H;
                            break;
                        case irany.jobbra:
                            tmp.x = VAC.getAktpoz().x + H;
                            tmp.y = VAC.getAktpoz().y;
                            break;
                        case irany.balra:
                            tmp.x = VAC.getAktpoz().x - H;
                            tmp.y = VAC.getAktpoz().y;
                            break;
                        default:
                            tmp.x = 10000;
                            tmp.y = 10000;
                            break;
                    }
                }
                /*
                if (VAC.getJartrect().Contains(VAC.GetFrontPoz())) //ha oda lépek ahol már jártam, egy esély-
                {
                    chance--;
                    if (chance == 0)
                        return;
                }
                */
            }
        }
        public async Task wallfollow_right_goaround()
        {
            //int chance = 10;
            VAC.getData();
            VACrajzol();
            while (VACutkozes() != 0) //ütközésig megy
            {
                VAC.getData();
                VAC.leptet();
                VACrajzol();
                await Task.Delay(move_milisec);
                VAC.getData();
            }
            bool quickright = false;
            VAC.balra();
            VAC.getData();
            if (VAC.getSensor()[0, 3] == ".")
            {
                quickright = true;
            }
            poz tmp;
            if (quickright)
            {
                VAC.jobbra();
                VAC.getData();
            }
            int jobbravizsgal = 0; // változó -> jó irányba történő vizsgáláshoz

            while (true)
            {
                if (alakzatvizsgal(VAC.getSensor()[0, 3]) == 0 && VACutkozes() != 0)
                {
                    VAC.getData();
                    VAC.leptet();
                    VACrajzol();
                    VAC.getData();
                    await Task.Delay(move_milisec);
                }
                else if (VACutkozes() == 0)
                {
                    VAC.getData();
                    VAC.balra();
                    VAC.getData();
                    if (VACutkozes() == 0)
                        continue;
                    VAC.leptet();
                    VACrajzol();
                    VAC.getData();
                    await Task.Delay(move_milisec);
                }
                else if (alakzatvizsgal(VAC.getSensor()[0, 3]) != 0 && VACutkozes() != 0)
                {
                    jobbravizsgal = 1;
                    VAC.getData();
                    VAC.jobbra();
                    VAC.getData();
                    if (VACutkozes() == 0)
                        continue;
                    VAC.leptet();
                    VACrajzol();
                    VAC.getData();
                    await Task.Delay(move_milisec);
                }
                if (jobbravizsgal == 1) // nem volt szinkronban a tmp vizsgálat és a robot léptetés
                {
                    VAC.jobbra(); //irányba állítás
                    switch (VAC.getIrany()) //vizsgálat
                    {
                        case irany.fel:
                            tmp.x = VAC.getAktpoz().x;
                            tmp.y = VAC.getAktpoz().y - H;
                            break;
                        case irany.le:
                            tmp.x = VAC.getAktpoz().x;
                            tmp.y = VAC.getAktpoz().y + H;
                            break;
                        case irany.jobbra:
                            tmp.x = VAC.getAktpoz().x + H;
                            tmp.y = VAC.getAktpoz().y;
                            break;
                        case irany.balra:
                            tmp.x = VAC.getAktpoz().x - H;
                            tmp.y = VAC.getAktpoz().y;
                            break;
                        default:
                            tmp.x = 10000;
                            tmp.y = 10000;
                            break;
                    }
                    VAC.balra(); //eredeti-vissza irányba állítás
                    jobbravizsgal = 0;
                }
                else //ha szinkronban volt a két művelet
                {
                    switch (VAC.getIrany())
                    {
                        case irany.fel:
                            tmp.x = VAC.getAktpoz().x;
                            tmp.y = VAC.getAktpoz().y - H;
                            break;
                        case irany.le:
                            tmp.x = VAC.getAktpoz().x;
                            tmp.y = VAC.getAktpoz().y + H;
                            break;
                        case irany.jobbra:
                            tmp.x = VAC.getAktpoz().x + H;
                            tmp.y = VAC.getAktpoz().y;
                            break;
                        case irany.balra:
                            tmp.x = VAC.getAktpoz().x - H;
                            tmp.y = VAC.getAktpoz().y;
                            break;
                        default:
                            tmp.x = 10000;
                            tmp.y = 10000;
                            break;
                    }
                }
                /*
                if (VAC.getJartrect().Contains(VAC.GetFrontPoz())) //ha oda lépek ahol már jártam, egy esély-
                {
                    chance--;
                    if (chance == 0)
                        return;
                }*/
            }
        }
        public async Task snake()
        {
            int chance = 2;
            int jobbrakigyo = 0;
            VAC.getData();
            VACrajzol();
            while (algdone == false)
            {
                while (VACutkozes() != 0) //ütközésig megy
                {
                    VAC.getData();
                    VAC.leptet();
                    VACrajzol();
                    await Task.Delay(move_milisec);
                    VAC.getData();
                }
                poz tmp; //balra vizsgálom, hogy voltam-e már ott
                switch (VAC.getIrany())
                {
                    case irany.fel:
                        tmp.x = VAC.getAktpoz().x - H; //balra mező
                        tmp.y = VAC.getAktpoz().y;
                        if (VAC.getJartrect().Contains(tmp))
                        {
                            VAC.jobbra();
                            jobbrakigyo = 1;
                        }
                        else
                        {
                            VAC.balra();
                            jobbrakigyo = 0;
                        }
                        VAC.getData();
                        if (VACutkozes() == 0)
                        {
                            algdone = true;
                            break;
                        }
                        VAC.leptet();
                        VACrajzol();
                        VAC.getData();
                        await Task.Delay(move_milisec);
                        if (jobbrakigyo == 1)
                        {
                            VAC.jobbra();
                        }
                        else
                        {
                            VAC.balra();
                        }
                        VAC.getData();
                        break;
                    case irany.le:
                        tmp.x = VAC.getAktpoz().x + H;
                        tmp.y = VAC.getAktpoz().y;
                        if (VAC.getJartrect().Contains(tmp))
                        {
                            VAC.jobbra();
                            jobbrakigyo = 1;
                        }
                        else
                        {
                            VAC.balra();
                            jobbrakigyo = 0;
                        }
                        VAC.getData();
                        if (VACutkozes() == 0)
                        {
                            algdone = true;
                            break;
                        }
                        VAC.leptet();
                        VACrajzol();
                        VAC.getData();
                        await Task.Delay(move_milisec);
                        if (jobbrakigyo == 1)
                        {
                            VAC.jobbra();
                        }
                        else
                        {
                            VAC.balra();
                        }
                        VAC.getData();
                        break;
                    case irany.jobbra:
                        tmp.x = VAC.getAktpoz().x;
                        tmp.y = VAC.getAktpoz().y - H;
                        if (VAC.getJartrect().Contains(tmp))
                        {
                            VAC.jobbra();
                            jobbrakigyo = 1;
                        }
                        else
                        {
                            VAC.balra();
                            jobbrakigyo = 0;
                        }
                        VAC.getData();
                        if (VACutkozes() == 0)
                        {
                            algdone = true;
                            break;
                        }
                        VAC.leptet();
                        VACrajzol();
                        VAC.getData();
                        await Task.Delay(move_milisec);
                        if (jobbrakigyo == 1)
                        {
                            VAC.jobbra();
                        }
                        else
                        {
                            VAC.balra();
                        }
                        VAC.getData();
                        break;
                    case irany.balra:
                        tmp.x = VAC.getAktpoz().x;
                        tmp.y = VAC.getAktpoz().y + H;
                        if (VAC.getJartrect().Contains(tmp))
                        {
                            VAC.jobbra();
                            jobbrakigyo = 1;
                        }
                        else
                        {
                            VAC.balra();
                            jobbrakigyo = 0;
                        }

                        VAC.getData();
                        if (VACutkozes() == 0)
                        {
                            algdone = true;
                            break;
                        }
                        VAC.leptet();
                        VACrajzol();
                        VAC.getData();
                        await Task.Delay(move_milisec);
                        if (jobbrakigyo == 1)
                        {
                            VAC.jobbra();
                        }
                        else
                        {
                            VAC.balra();

                        }
                        VAC.getData();
                        break;
                }
                if (VAC.getJartrect().Contains(VAC.getAktpoz())) //ha oda lépek ahol már jártam, egy esély-
                {
                    chance--;
                    if (chance == 0)
                        break;
                }
            }
        }
        public async Task snake2_right()
        {
            int chance = 10;
            VAC.getData();
            VACrajzol();
            while (true)
            {
                while (VACutkozes() != 0) //mindig ütközésig megy, többi kód csak fordulás
                {
                    poz tmp_bal;
                    poz tmp_jobb;
                    poz tmp_fel;
                    poz tmp_le;
                    tmp_bal.x = 0;
                    tmp_bal.y = 0;
                    tmp_jobb.x = 0;
                    tmp_jobb.y = 0;
                    tmp_fel.x = 0;
                    tmp_fel.y = 0;
                    tmp_le.x = 0;
                    tmp_le.y = 0;
                    VAC.getData();
                    VAC.leptet();
                    VACrajzol();
                    await Task.Delay(move_milisec);
                    if (VAC.getIrany() == irany.fel)
                    {
                        tmp_bal.x = VAC.getAktpoz().x - H;
                        tmp_bal.y = VAC.getAktpoz().y - W;
                        tmp_jobb.x = VAC.getAktpoz().x + H;
                        tmp_jobb.y = VAC.getAktpoz().y -W;
                    }
                    if (VAC.getIrany() == irany.le)
                    {
                        tmp_bal.x = VAC.getAktpoz().x - H;
                        tmp_bal.y = VAC.getAktpoz().y + W;
                        tmp_jobb.x = VAC.getAktpoz().x + H;
                        tmp_jobb.y = VAC.getAktpoz().y + W;
                    }
                    if (VAC.getIrany() == irany.balra)
                    {
                        tmp_fel.x = VAC.getAktpoz().x - H;
                        tmp_fel.y = VAC.getAktpoz().y - W;
                        tmp_le.x = VAC.getAktpoz().x - H;
                        tmp_le.y = VAC.getAktpoz().y + W;
                    }
                    if (VAC.getIrany() == irany.jobbra)
                    {
                        tmp_fel.x = VAC.getAktpoz().x + H;
                        tmp_fel.y = VAC.getAktpoz().y - W;
                        tmp_le.x = VAC.getAktpoz().x + H;
                        tmp_le.y = VAC.getAktpoz().y + W;
                    }
                    if (VAC.getIrany() == irany.fel)
                    {
                        if (!falakpoz.Contains(tmp_bal) && !falakpoz.Contains(tmp_jobb))
                        {
                            if (VAC.getJartrect().Contains(tmp_bal))
                            {
                                which_leftright--;
                            }
                            if (VAC.getJartrect().Contains(tmp_jobb))
                            {
                                which_leftright++;
                            }
                        }                    
                    }
                    if (VAC.getIrany() == irany.le)
                    {
                        if (!falakpoz.Contains(tmp_bal) && !falakpoz.Contains(tmp_jobb))
                        {
                            if (VAC.getJartrect().Contains(tmp_bal))
                            {
                                which_leftright--;
                            }
                            if (VAC.getJartrect().Contains(tmp_jobb))
                            {
                                which_leftright++;
                            }
                        }                      
                    }
                    if (VAC.getIrany() == irany.jobbra)
                    {
                        if (!falakpoz.Contains(tmp_fel) && !falakpoz.Contains(tmp_le))
                        {
                            if (VAC.getJartrect().Contains(tmp_fel))
                            {
                                which_leftright--;
                            }
                            if (VAC.getJartrect().Contains(tmp_le))
                            {
                                which_leftright++;
                            }
                        }                        
                    }
                    if (VAC.getIrany() == irany.balra)
                    {
                        if (!falakpoz.Contains(tmp_fel) && !falakpoz.Contains(tmp_le))
                        {
                            if (VAC.getJartrect().Contains(tmp_fel))
                            {
                                which_leftright--;
                            }
                            if (VAC.getJartrect().Contains(tmp_le))
                            {
                                which_leftright++;
                            }
                        }                          
                    }
                    VAC.getData();
                    if (lefedettség.Value == lefedettség.Maximum)
                        return;
                    if (VAC.getJartrect().Contains(VAC.GetFrontPoz())) //ha oda lépek ahol már jártam, egy esély-
                    {
                        chance--;
                        if (chance == 0)
                        {
                            which_leftright = 0;
                            sign_nochance = 1;
                            return;
                        }
                    }
                }
                if (VACutkozes() == 0)
                {
                    if (which_leftright > 0)
                    {
                        break;
                    }
                }
                which_leftright = 0;
                switch (VAC.getIrany())
                {
                    case irany.fel:
                        VAC.jobbra();
                        VAC.getData();
                        if (VACutkozes() == 0)
                        {
                            VAC.balra();
                            VAC.balra();
                            VAC.getData();
                            continue;
                        }
                        VAC.leptet();
                        VACrajzol();
                        VAC.getData();
                        await Task.Delay(move_milisec);
                        VAC.jobbra();
                        VAC.getData();
                        break;
                    case irany.le:
                        VAC.balra();
                        VAC.getData();
                        if (VACutkozes() == 0)
                        {
                            VAC.jobbra();
                            VAC.jobbra();
                            VAC.getData();
                            continue;
                        }
                        VAC.leptet();
                        VACrajzol();
                        VAC.getData();
                        await Task.Delay(move_milisec);
                        VAC.balra();
                        VAC.getData();
                        break;
                    case irany.jobbra:
                        VAC.jobbra();
                        VAC.getData();
                        if (VACutkozes() == 0)
                        {
                            VAC.balra();
                            VAC.balra();
                            VAC.getData();
                            continue;
                        }
                        VAC.leptet();
                        VACrajzol();
                        VAC.getData();
                        await Task.Delay(move_milisec);
                        VAC.jobbra();
                        VAC.getData();
                        break;
                    case irany.balra:
                        VAC.balra();
                        VAC.getData();
                        if (VACutkozes() == 0)
                        {
                            VAC.jobbra();
                            VAC.jobbra();
                            VAC.getData();
                            continue;
                        }
                        VAC.leptet();
                        VACrajzol();
                        VAC.getData();
                        await Task.Delay(move_milisec);
                        VAC.balra();
                        VAC.getData();
                        break;
                }
            }
        }
        public async Task snake2_left()
        {
            int chance = 10;
            VAC.getData();
            VACrajzol();
            while (true)
            {
                while (VACutkozes() != 0) //mindig ütközésig megy, többi kód csak fordulás
                {
                    poz tmp_bal;
                    poz tmp_jobb;
                    poz tmp_fel;
                    poz tmp_le;
                    tmp_bal.x = 0;
                    tmp_bal.y = 0;
                    tmp_jobb.x = 0;
                    tmp_jobb.y = 0;
                    tmp_fel.x = 0;
                    tmp_fel.y = 0;
                    tmp_le.x = 0;
                    tmp_le.y = 0;
                    VAC.getData();
                    VAC.leptet();
                    VACrajzol();
                    await Task.Delay(move_milisec);
                    if (VAC.getIrany() == irany.fel)
                    {
                        tmp_bal.x = VAC.getAktpoz().x - H;
                        tmp_bal.y = VAC.getAktpoz().y - W;
                        tmp_jobb.x = VAC.getAktpoz().x + H;
                        tmp_jobb.y = VAC.getAktpoz().y - W;
                    }
                    if (VAC.getIrany() == irany.le)
                    {
                        tmp_bal.x = VAC.getAktpoz().x - H;
                        tmp_bal.y = VAC.getAktpoz().y + W;
                        tmp_jobb.x = VAC.getAktpoz().x + H;
                        tmp_jobb.y = VAC.getAktpoz().y + W;
                    }
                    if (VAC.getIrany() == irany.balra)
                    {
                        tmp_fel.x = VAC.getAktpoz().x - H;
                        tmp_fel.y = VAC.getAktpoz().y - W;
                        tmp_le.x = VAC.getAktpoz().x - H;
                        tmp_le.y = VAC.getAktpoz().y + W;
                    }
                    if (VAC.getIrany() == irany.jobbra)
                    {
                        tmp_fel.x = VAC.getAktpoz().x + H;
                        tmp_fel.y = VAC.getAktpoz().y - W;
                        tmp_le.x = VAC.getAktpoz().x + H;
                        tmp_le.y = VAC.getAktpoz().y + W;
                    }
                    if (VAC.getIrany() == irany.fel)
                    {
                        if (!falakpoz.Contains(tmp_bal) && !falakpoz.Contains(tmp_jobb))
                        {
                            if (VAC.getJartrect().Contains(tmp_bal))
                            {
                                which_leftright--;
                            }
                            if (VAC.getJartrect().Contains(tmp_jobb) && !falakpoz.Contains(tmp_jobb))
                            {
                                which_leftright++;
                            }
                        }                            
                    }
                    if (VAC.getIrany() == irany.le)
                    {
                        if (!falakpoz.Contains(tmp_bal) && !falakpoz.Contains(tmp_jobb))
                        {
                            if (VAC.getJartrect().Contains(tmp_bal))
                            {
                                which_leftright--;
                            }
                            if (VAC.getJartrect().Contains(tmp_jobb))
                            {
                                which_leftright++;
                            }
                        }                        
                    }
                    if (VAC.getIrany() == irany.jobbra)
                    {
                        if (!falakpoz.Contains(tmp_bal) && !falakpoz.Contains(tmp_jobb))
                        {
                            if (VAC.getJartrect().Contains(tmp_fel))
                            {
                                which_leftright--;
                            }
                            if (VAC.getJartrect().Contains(tmp_le))
                            {
                                which_leftright++;
                            }
                        }                        
                    }
                    if (VAC.getIrany() == irany.balra)
                    {
                        if (!falakpoz.Contains(tmp_bal) && !falakpoz.Contains(tmp_jobb))
                        {
                            if (VAC.getJartrect().Contains(tmp_fel))
                            {
                                which_leftright--;
                            }
                            if (VAC.getJartrect().Contains(tmp_le))
                            {
                                which_leftright++;
                            }
                        }               
                    }
                    VAC.getData();
                    if (lefedettség.Value == lefedettség.Maximum)
                        return;
                    if (VAC.getJartrect().Contains(VAC.GetFrontPoz())) //ha oda lépek ahol már jártam, egy esély-
                    {
                        chance--;
                        if (chance == 0)
                        {
                            which_leftright = 0;
                            sign_nochance = 1;
                            return;
                        }
                    }
                }
                if (VACutkozes() == 0)
                { 
                    if (which_leftright < 0)
                    {
                        break;
                    }
                }
                which_leftright = 0;
                    switch (VAC.getIrany())
                    {
                        case irany.fel:
                            VAC.balra();
                            VAC.getData();
                            if (VACutkozes() == 0)
                            {
                                VAC.jobbra();
                                VAC.jobbra();
                                VAC.getData();
                                continue;
                            }
                            VAC.leptet();
                            VACrajzol();
                            VAC.getData();
                            await Task.Delay(move_milisec);
                            VAC.balra();
                            VAC.getData();
                            break;
                        case irany.le:
                            VAC.jobbra();
                            VAC.getData();
                            if (VACutkozes() == 0)
                            {
                                VAC.balra();
                                VAC.balra();
                                VAC.getData();
                                continue;
                            }
                            VAC.leptet();
                            VACrajzol();
                            VAC.getData();
                            await Task.Delay(move_milisec);
                            VAC.jobbra();
                            VAC.getData();
                            break;
                        case irany.jobbra:
                            VAC.balra();
                            VAC.getData();
                            if (VACutkozes() == 0)
                            {
                                VAC.jobbra();
                                VAC.jobbra();
                                VAC.getData();
                                continue;
                            }
                            VAC.leptet();
                            VACrajzol();
                            VAC.getData();
                            await Task.Delay(move_milisec);
                            VAC.balra();
                            VAC.getData();
                            break;
                        case irany.balra:
                            VAC.jobbra();
                            VAC.getData();
                            if (VACutkozes() == 0)
                            {
                                VAC.balra();
                                VAC.balra();
                                VAC.getData();
                                continue;
                            }
                            VAC.leptet();
                            VACrajzol();
                            VAC.getData();
                            await Task.Delay(move_milisec);
                            VAC.jobbra();
                            VAC.getData();
                            break;
                    }
            }
        }
        public async Task escape() 
        {
            int washere1 = 0;
            int washere2 = 0;
            counter_washere = 0;
            poz towhere;
            VAC.getData();
            poz aktpozIndex = VAC.getAktpoz();
            towhere = VAC.smallestdistanceindex(); //ez a legközelebbi be nem járt pozíció
            //adott pozícióra a porszívó eljuttatása:
            counter_firstcheck = 0;
            while (aktpozIndex.x != towhere.x || aktpozIndex.y != towhere.y)
            {
                if (lefedettség.Value == lefedettség.Maximum)
                    return;
                VAC.getData();
                poz DirVector;
                aktpozIndex = VAC.convertAktpozToIndex();
                DirVector.x = towhere.x - aktpozIndex.x;
                DirVector.y = towhere.y - aktpozIndex.y;
                if (DirVector.x == 0 && DirVector.y == 0)
                {
                    break;
                }
                if (VACutkozes() == 0)
                {
                    if (counter_firstcheck == 0 || (washere1 == 1 && washere2 == 1))
                    {
                        check = VAC.getAktpoz();
                        counter_firstcheck++;
                    }
                    switch (VAC.getIrany())
                    { 
                        case irany.fel:
                            {
                                if (check.x == VAC.getAktpoz().x && check.y == VAC.getAktpoz().y)
                                    counter_washere++;
                                if (counter_washere == 2)
                                {
                                    await wallfollow_right_escape();
                                    counter_washere = 0;
                                    washere1 = 1;
                                }
                                else
                                {
                                    await wallfollow_left_escape();
                                    washere2 = 1;
                                }
                                break;
                            }
                        case irany.le:
                            {
                                if (check.x == VAC.getAktpoz().x && check.y == VAC.getAktpoz().y)
                                    counter_washere++;
                                if (counter_washere == 2)
                                {
                                    await wallfollow_left_escape();
                                    counter_washere = 0;
                                }
                                else
                                {
                                    await wallfollow_right_escape();
                                }
                                break;
                            }
                        case irany.jobbra:
                            {
                                if (check.x == VAC.getAktpoz().x && check.y == VAC.getAktpoz().y)
                                    counter_washere++;
                                if (counter_washere == 2)
                                {
                                    await wallfollow_left_escape();
                                    counter_washere = 0;
                                }
                                else
                                {
                                    await wallfollow_right_escape();
                                }
                                break;
                            }
                        case irany.balra:
                            {
                                if (check.x == VAC.getAktpoz().x && check.y == VAC.getAktpoz().y)
                                    counter_washere++;
                                if (counter_washere == 2)
                                {
                                    await wallfollow_right_escape();
                                    counter_washere = 0;
                                }
                                else
                                {
                                    await wallfollow_left_escape();
                                }
                                break;
                            }
                    }
                }
                VAC.getData();
                aktpozIndex = VAC.convertAktpozToIndex();
                DirVector.x = towhere.x - aktpozIndex.x;
                DirVector.y = towhere.y - aktpozIndex.y;
                if (DirVector.x == 0 && DirVector.y == 0)
                {
                    break;
                }
                while (DirVector.x > 0)
                {
                    VAC.iranyvalt(irany.jobbra); //jobbra megyek
                    VAC.getData();
                    if (VACutkozes() == 0)
                        break;
                    VAC.leptet();
                    DirVector.x--;
                    VACrajzol();
                    await Task.Delay(move_milisec);
                    VAC.getData();
                }
                while (DirVector.x < 0)
                {
                    VAC.iranyvalt(irany.balra); //balra megyek
                    VAC.getData();
                    if (VACutkozes() == 0)
                        break;
                    VAC.leptet();
                    DirVector.x++;
                    VACrajzol();
                    await Task.Delay(move_milisec);
                    VAC.getData();
                }
                while (DirVector.y > 0)
                {
                    VAC.iranyvalt(irany.le); //le megyek
                    VAC.getData();
                    if (VACutkozes() == 0)
                        break;
                    VAC.leptet();
                    DirVector.y--;
                    VACrajzol();
                    await Task.Delay(move_milisec);
                    VAC.getData();
                }
                while (DirVector.y < 0)
                {
                    VAC.iranyvalt(irany.fel); //fel megyek
                    VAC.getData();
                    if (VACutkozes() == 0)
                        break;
                    VAC.leptet();
                    DirVector.y++;
                    VACrajzol();
                    await Task.Delay(move_milisec);
                    VAC.getData();
                }
            }
        }

        //EVENTEK

        //pálya inicializálásáért felelős CLICK-event
        private void Button_Click(object sender, RoutedEventArgs e) 
        {
            int x = 4 * H;
            int y = H;
            StreamReader reader = new StreamReader(room);
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
                        poz fal;
                        fal.x = x;
                        fal.y = y;
                        falakpoz.Add(fal);
                    }
                    else if (alakzatvizsgal(palya[i, j]) == 1)
                    {

                        robotrajzol(x, y);
                        robotpoz.x = x;
                        robotpoz.y = y;
                        VAC.setAktpoz(robotpoz);
                    }
                    else if (alakzatvizsgal(palya[i, j]) == 2)
                    {
                        szobaterulet++;
                    }
                    x += W;
                }
                if (x == (SOR + 4) * H)
                    x = 4 * H;
                y += H;
            }
        }

        //algoritmus CLICK-eventek
        private void alg1_Click(object sender, RoutedEventArgs e)
        {
            alg_random_egyszererint();
        }
        private void alg2_Click(object sender, RoutedEventArgs e)
        {
            wallfollow_right_goaround();
        }
        private async void alg3_Click(object sender, RoutedEventArgs e)
        {
            await circle();
        }
        private async void alg4_Click(object sender, RoutedEventArgs e)
        {
            await snake2_left();
        }
        private async void alg5_Click(object sender, RoutedEventArgs e)
        {
            await wallfollow_left_goaround();
            await Task.Delay(think_milisec);
            await Task.Delay(think_milisec);
            await escape();
            await Task.Delay(think_milisec);
            await Task.Delay(think_milisec);
            while (lefedettség.Value != lefedettség.Maximum)
            {
                 while (true)
                 {
                     if (lefedettség.Value == lefedettség.Maximum)
                     {
                         return;
                     }
                     if (sign_nochance == 1)
                     {
                         sign_nochance = 0;
                         break;
                     }
                     if (which_leftright <= 0)
                     {
                         await snake2_right();
                     }
                     if (which_leftright > 0)
                     {
                         await snake2_left();
                     }
                 }
                await Task.Delay(think_milisec);
                await Task.Delay(think_milisec);
                await escape();
                await Task.Delay(think_milisec);
                await Task.Delay(think_milisec);
            }
        }

        //szoba-választó CLICK-event
        private void roomselect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string roomName = e.AddedItems[0].ToString();
            switch (roomName)
            {
                case "Szoba 1":
                    room = "robot.txt";
                    break;
                case "Szoba 2":
                    room = "robot2.txt";
                    break;
                case "Szoba 3":
                    room = "robot3.txt";
                    break;
                default:
                    break;
            }
        }

        //a robot sebességért felelős CLICK-eventek
        private void speed_plus_Click(object sender, RoutedEventArgs e)
        {
            move_milisec = move_milisec - 25;
            think_milisec = think_milisec - 150;
        }

        private void speed_minus_Click(object sender, RoutedEventArgs e)
        {
            move_milisec = move_milisec + 25;
            think_milisec = think_milisec + 150;
        }

        //a robot manuális mozgatásához a CLICK-eventek
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

        //a progressbar változtatásáért felelős VALUECHANGED-event
        private void lefedettség_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            lefedettség.Maximum = szobaterulet;
            lefedettség.Value = progressbarint;
        }

        private void Restart_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}