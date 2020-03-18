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
          string[,] getData();
    }

   /* public interface Irobotoutput
    {
        void leptet(irany nez);
    }
    */
    public class VacCleaner : Irobotinput /*, Irobotoutput */
    {

        public VacCleaner(string [,] palya, poz aktpoz, irany nez)
        {
            this.palya = palya;
            this.aktpoz = aktpoz;
            this.aktirany = nez;
        }
        
        string[,] palya;
        poz aktpoz;
        irany aktirany;

        // A output interface még nem működik
          
        public void leptet(irany nez)
        {
            switch (nez)
            {
                case irany.fel:
                    robotfel(this.aktpoz);
                    break;
                case irany.le:
                    robotle(this.aktpoz);
                    break;
                case irany.jobbra:
                    robotjobb(this.aktpoz);
                    break;
                case irany.balra:
                    robotbal(this.aktpoz);
                    break;
            }
            
        }

    

        public string[,] getData ()
        {
            string[,] tmp = new string[2, 5];
            int k = 0;
            int l = 0;
            switch (this.aktirany)
            {
                case irany.fel:
                    {
                        int oszlop = (this.aktpoz.x - 200) / 50 - 2 ;
                        int sor = (this.aktpoz.y - 50) / 50 - 1;
                        for (int i =  sor; i > sor - 2; i--)
                        {
                            for (int j = oszlop; j < oszlop + 5; j++)
                            {
                                    tmp[k, l] = palya[i, j];
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
            return tmp;
        }
    }
}
