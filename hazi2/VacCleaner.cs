using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static hazi2.MainPage;

namespace hazi2
{

    public interface Irobotinput
    {
        void getData();
    }

    public interface Irobotoutput
    {
        void leptet();
        void iranyvalt(irany nez);
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
        public void setAktpoz(poz p)
        {
            this.aktpoz = p;
        }
        public poz getAktpoz()
        {
            return this.aktpoz;
        }

        //tagfüggvények

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



        public void getData()
        {
            string[,] tmp = new string[2, 5];
            int k = 0;
            int l = 0;
            int kindex = 0;
            int lindex = 0;
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
                                tmp[k, l] = palya[i, j];
                                mem[i, j] = palya[i, j];
                                l++;
                            }
                            l = 0;
                            k++;
                        }
                        tmp[0, 0] = "?";
                        tmp[0, 4] = "?";
                        if (tmp[0, 1] == "o")
                            tmp[1, 0] = "?";
                        if (tmp[0, 2] == "o")
                            tmp[1, 2] = "?";
                        if (tmp[0, 3] == "o")
                            tmp[1, 4] = "?";
                        if (tmp[0, 1] == "o" && tmp[0, 2] == "o")
                            tmp[1, 1] = "?";
                        if (tmp[0, 2] == "o" && tmp[0, 3] == "o")
                            tmp[1, 3] = "?";
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

                                tmp[k, l] = palya[i, j];
                                l++;
                                mem[i, j] = palya[i, j];
                            }
                            l = 0;
                            k++;
                        }
                        tmp[0, 0] = "?";
                        tmp[0, 4] = "?";
                        if (tmp[0, 1] == "o")
                            tmp[1, 0] = "?";
                        if (tmp[0, 2] == "o")
                            tmp[1, 2] = "?";
                        if (tmp[0, 3] == "o")
                            tmp[1, 4] = "?";
                        if (tmp[0, 1] == "o" && tmp[0, 2] == "o")
                            tmp[1, 1] = "?";
                        if (tmp[0, 2] == "o" && tmp[0, 3] == "o")
                            tmp[1, 3] = "?";
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

                                tmp[k, l] = palya[i, j];
                                mem[i, j] = palya[i, j];
                                l++;
                            }
                            l = 0;
                            k++;
                        }
                        tmp[0, 0] = "?";
                        tmp[0, 4] = "?";
                        if (tmp[0, 1] == "o")
                            tmp[1, 0] = "?";
                        if (tmp[0, 2] == "o")
                            tmp[1, 2] = "?";
                        if (tmp[0, 3] == "o")
                            tmp[1, 4] = "?";
                        if (tmp[0, 1] == "o" && tmp[0, 2] == "o")
                            tmp[1, 1] = "?";
                        if (tmp[0, 2] == "o" && tmp[0, 3] == "o")
                            tmp[1, 3] = "?";
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

                                tmp[k, l] = palya[i, j];
                                mem[i, j] = palya[i, j];
                                l++;
                            }
                            l = 0;
                            k++;
                        }
                        tmp[0, 0] = "?";
                        tmp[0, 4] = "?";
                        if (tmp[0, 1] == "o")
                            tmp[1, 0] = "?";
                        if (tmp[0, 2] == "o")
                            tmp[1, 2] = "?";
                        if (tmp[0, 3] == "o")
                            tmp[1, 4] = "?";
                        if (tmp[0, 1] == "o" && tmp[0, 2] == "o")
                            tmp[1, 1] = "?";
                        if (tmp[0, 2] == "o" && tmp[0, 3] == "o")
                            tmp[1, 3] = "?";
                    }
                    break;
            }



        }
    }

}
