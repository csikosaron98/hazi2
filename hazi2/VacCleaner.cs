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

        const int SOR = 12;
        const int OSZLOP = 12;
        const int H = 50;
        const int W = 50;

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
                    tmp.y = tmp.y - 50;
                    break;
                case irany.le:
                    tmp.y = tmp.y + 50;
                    break;
                case irany.jobbra:
                    tmp.x = tmp.x + 50;
                    break;
                case irany.balra:
                    tmp.x = tmp.x - 50;
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
        public poz convert()
        {
            int x = (getAktpoz().x - 200) / 50;
            int y = (getAktpoz().y - 50) / 50;
            poz tmp;
            tmp.x = x;
            tmp.y = y;
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
                        int oszlop = (this.aktpoz.x - 200) / 50 - 2;
                        int sor = (this.aktpoz.y - 50) / 50 - 1;
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
                        int oszlop = (this.aktpoz.x - 200) / 50 + 2;
                        int sor = (this.aktpoz.y - 50) / 50 + 1;
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
                        int oszlop = (this.aktpoz.x - 200) / 50 + 1;
                        int sor = (this.aktpoz.y - 50) / 50 - 2;
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
                        int oszlop = (this.aktpoz.x - 200) / 50 - 1;
                        int sor = (this.aktpoz.y - 50) / 50 + 2;
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
    }
}
