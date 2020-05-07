using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static hazi2.MainPage;


namespace hazi2
{

    // két Interface: bemenet(szenzor), kimenet(motor)
    public interface Irobotinput
    {
        void getData();
    }

    public interface Irobotoutput
    {
        void leptet();
        void iranyvalt(irany nez);
        void jobbra();
        void balra();
    }

    public class VacCleaner : Irobotinput, Irobotoutput
    {
        public VacCleaner(string[,] palya, poz aktpoz, irany nez)
        {
            this.palya = palya;
            this.aktpoz = aktpoz;
            this.aktirany = nez;
        }

        //konstansok

        const int SOR = 15;
        const int OSZLOP = 15;
        const int H = 45;
        const int W = 45;

        //tagváltozók

        string[,] palya;
        poz aktpoz;
        irany aktirany;
        static string[,] mem = new string[SOR, OSZLOP];
        static string[,] sensor = new string[2, 5];
        List<poz> Jartrect = new List<poz>();

        //setterek, getterek

        public void setAktpoz(poz p)
        {
            this.aktpoz = p;
        }
        public poz GetFrontPoz()
        {
            poz tmp = this.getAktpoz();
            switch (aktirany)
            {
                case irany.fel:
                    tmp.y = tmp.y - H;
                    break;
                case irany.le:
                    tmp.y = tmp.y + H;
                    break;
                case irany.jobbra:
                    tmp.x = tmp.x + H;
                    break;
                case irany.balra:
                    tmp.x = tmp.x - H;
                    break;

            }
            return tmp;
        }
        public poz getAktpoz()
        {
            return this.aktpoz;
        }
        public List<poz> getJartrect()
        {
            return this.Jartrect;
        }
        public string[,] getSensor()
        {
            return sensor;
        }
        public string[,] getMem()
        {
            return mem;
        }
        public irany getIrany()
        {
            return this.aktirany;
        }


        //tagfüggvények
        public poz convertAktpozToIndex()
        {
            int x = (getAktpoz().x - 4*H) / H;
            int y = (getAktpoz().y - H) / H;
            poz tmp;
            tmp.x = x;
            tmp.y = y;
            return tmp;
        }
        public poz convertIndexToPoz(int x, int y)
        {
            poz tmp;
            tmp.x = x * H + 4*H;
            tmp.y = y * H + H;
            return tmp;
        }

        public void leptet()
        {
            switch (aktirany)
            {
                case irany.fel:
                    this.aktpoz.y = this.aktpoz.y - H;
                    break;
                case irany.le:
                    this.aktpoz.y = this.aktpoz.y + H;
                    break;
                case irany.jobbra:
                    this.aktpoz.x = this.aktpoz.x + W;
                    break;
                case irany.balra:
                    this.aktpoz.x = this.aktpoz.x - W;
                    break;
            }
        }
        public void iranyvalt(irany nez)
        {
            this.aktirany = nez;
        }
        public void jobbra()
        {
            switch (aktirany)
            {
                case irany.fel:
                    {
                        this.iranyvalt(irany.jobbra);
                    }
                    break;
                case irany.le:
                    {
                        this.iranyvalt(irany.balra);
                    }
                    break;
                case irany.jobbra:
                    {
                        this.iranyvalt(irany.le);
                    }
                    break;
                case irany.balra:
                    {
                        this.iranyvalt(irany.fel);
                    }
                    break;
            }
        }
        public void balra()
        {
            switch (aktirany)
            {
                case irany.fel:
                    {
                        this.iranyvalt(irany.balra);
                    }
                    break;
                case irany.le:
                    {
                        this.iranyvalt(irany.jobbra);
                    }
                    break;
                case irany.jobbra:
                    {
                        this.iranyvalt(irany.fel);
                    }
                    break;
                case irany.balra:
                    {
                        this.iranyvalt(irany.le);
                    }
                    break;
            }
        }
        public void getData()
        {
            int k = 0;
            int l = 0;
            switch (aktirany)
            {
                case irany.fel:
                    {
                        int oszlop = (this.aktpoz.x - 4*H) / H - 2;
                        int sor = (this.aktpoz.y - H) / H - 1;
                        for (int i = sor; i > sor - 2; i--)
                        {
                            for (int j = oszlop; j < oszlop + 5; j++)
                            {
                                sensor[k, l] = palya[i, j];
                                mem[i, j] = palya[i, j];
                                l++;
                            }
                            l = 0;
                            k++;
                        }
                        sensor[0, 0] = "?";
                        sensor[0, 4] = "?";
                        if (sensor[0, 1] == "o")
                            sensor[1, 0] = "?";
                        if (sensor[0, 2] == "o")
                            sensor[1, 2] = "?";
                        if (sensor[0, 3] == "o")
                            sensor[1, 4] = "?";
                        if (sensor[0, 1] == "o" && sensor[0, 2] == "o")
                            sensor[1, 1] = "?";
                        if (sensor[0, 2] == "o" && sensor[0, 3] == "o")
                            sensor[1, 3] = "?";
                    }
                    break;
                case irany.le:
                    {
                        int oszlop = (this.aktpoz.x - 4*H) / H + 2;
                        int sor = (this.aktpoz.y - H) / H + 1;
                        for (int i = sor; i < sor + 2; i++)
                        {
                            for (int j = oszlop; j > oszlop - 5; j--)
                            {

                                sensor[k, l] = palya[i, j];
                                l++;
                                mem[i, j] = palya[i, j];
                            }
                            l = 0;
                            k++;
                        }
                        sensor[0, 0] = "?";
                        sensor[0, 4] = "?";
                        if (sensor[0, 1] == "o")
                            sensor[1, 0] = "?";
                        if (sensor[0, 2] == "o")
                            sensor[1, 2] = "?";
                        if (sensor[0, 3] == "o")
                            sensor[1, 4] = "?";
                        if (sensor[0, 1] == "o" && sensor[0, 2] == "o")
                            sensor[1, 1] = "?";
                        if (sensor[0, 2] == "o" && sensor[0, 3] == "o")
                            sensor[1, 3] = "?";
                    }
                    break;
                case irany.jobbra:
                    {
                        int oszlop = (this.aktpoz.x - 4*H) / H + 1;
                        int sor = (this.aktpoz.y - H) / H - 2;
                        for (int j = oszlop; j < oszlop + 2; j++)
                        {
                            for (int i = sor; i < sor + 5; i++)
                            {

                                sensor[k, l] = palya[i, j];
                                mem[i, j] = palya[i, j];
                                l++;
                            }
                            l = 0;
                            k++;
                        }
                        sensor[0, 0] = "?";
                        sensor[0, 4] = "?";
                        if (sensor[0, 1] == "o")
                            sensor[1, 0] = "?";
                        if (sensor[0, 2] == "o")
                            sensor[1, 2] = "?";
                        if (sensor[0, 3] == "o")
                            sensor[1, 4] = "?";
                        if (sensor[0, 1] == "o" && sensor[0, 2] == "o")
                            sensor[1, 1] = "?";
                        if (sensor[0, 2] == "o" && sensor[0, 3] == "o")
                            sensor[1, 3] = "?";
                    }
                    break;
                case irany.balra:
                    {
                        int oszlop = (this.aktpoz.x - 4*H) / H - 1;
                        int sor = (this.aktpoz.y - H) / H + 2;
                        for (int j = oszlop; j > oszlop - 2; j--)
                        {
                            for (int i = sor; i > sor - 5; i--)
                            {

                                sensor[k, l] = palya[i, j];
                                mem[i, j] = palya[i, j];
                                l++;
                            }
                            l = 0;
                            k++;
                        }
                        sensor[0, 0] = "?";
                        sensor[0, 4] = "?";
                        if (sensor[0, 1] == "o")
                            sensor[1, 0] = "?";
                        if (sensor[0, 2] == "o")
                            sensor[1, 2] = "?";
                        if (sensor[0, 3] == "o")
                            sensor[1, 4] = "?";
                        if (sensor[0, 1] == "o" && sensor[0, 2] == "o")
                            sensor[1, 1] = "?";
                        if (sensor[0, 2] == "o" && sensor[0, 3] == "o")
                            sensor[1, 3] = "?";
                    }
                    break;
            }
        }
        public poz smallestdistanceindex() //mátrixon belüli index-szel tér vissza
        {
            double smallest = 1000;
            poz tmp;
            poz index;
            index.x = 0;
            index.y = 0;
            double valuetmp = 0;
            for (int i = 0; i < SOR; i++)
            {
                for (int j = 0; j < OSZLOP; j++)
                {
                    if (mem[i, j] == ".") //ahol nincs fal
                    {
                        poz tmp2;
                        tmp2.x = j;
                        tmp2.y = i;
                        tmp2 = convertIndexToPoz(j, i);
                        if (!Jartrect.Contains(tmp2))
                        {
                            tmp = convertAktpozToIndex(); //aktpoz konvertálás
                            valuetmp = distance(j, i, tmp.x, tmp.y);
                            if (valuetmp < smallest)
                            {
                                smallest = valuetmp;
                                index.x = j;
                                index.y = i;
                            }
                        }

                    }
                }
            }
            return index;
        }
        double distance(int x1, int y1, int x2, int y2)
        {
            return Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        }

        public poz nearestindex() //mátrixon belüli index-szel tér vissza
        {
            double smallest = 1000;
            double secondsmallest = 1000;
            poz tmp;
            poz index;
            index.x = 0;
            index.y = 0;
            double valuetmp = 0;
            for (int i = 0; i < SOR; i++)
            {
                for (int j = 0; j < OSZLOP; j++)
                {
                    if (mem[i, j] == ".") //ahol nincs fal
                    {
                        tmp = convertAktpozToIndex(); //aktpoz konvertálás
                        valuetmp = distance(j, i, tmp.x, tmp.y);
                        if (valuetmp < smallest)
                        {
                           secondsmallest = smallest;
                           smallest = valuetmp;
                        }
                        else if (valuetmp < secondsmallest && valuetmp != smallest)
                        {
                            secondsmallest = valuetmp;
                        }

                    }
                }
            }
            for (int i = 0; i < SOR; i++)
            {
                for (int j = 0; j < OSZLOP; j++)
                {
                    if (mem[i, j] == ".") //ahol nincs fal
                    {
                        tmp = convertAktpozToIndex(); //aktpoz konvertálás
                        valuetmp = distance(j, i, tmp.x, tmp.y);
                        if (valuetmp == secondsmallest)
                        {
                            index.x = j;
                            index.y = i;
                        }

                    }
                }
            }
            return index;
        }
        
    }
}
