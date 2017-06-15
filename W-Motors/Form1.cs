using Bike18;
using OfficeOpenXml;
using RacerMotors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace W_Motors
{
    public partial class Form1 : Form
    {
        Thread forms;

        nethouse nethouse = new nethouse();
        WebClient webClient = new WebClient();
        httpRequest httprequest = new httpRequest();
        FileEdit files = new FileEdit();

        string minitextTemplate;
        string fullTextTemplate;
        string keywordsTextTemplate;
        string titleTextTemplate;
        string descriptionTextTemplate;

        string fileUrls;

        public Form1()
        {
            InitializeComponent();
            if (!Directory.Exists("files"))
            {
                Directory.CreateDirectory("files");
            }
            if (!Directory.Exists("pic"))
            {
                Directory.CreateDirectory("pic");
            }

            if (!File.Exists("files\\miniText.txt"))
            {
                File.Create("files\\miniText.txt");
            }

            if (!File.Exists("files\\fullText.txt"))
            {
                File.Create("files\\fullText.txt");
            }

            if (!File.Exists("files\\title.txt"))
            {
                File.Create("files\\title.txt");
            }

            if (!File.Exists("files\\description.txt"))
            {
                File.Create("files\\description.txt");
            }

            if (!File.Exists("files\\keywords.txt"))
            {
                File.Create("files\\keywords.txt");
            }
            StreamReader altText = new StreamReader("files\\miniText.txt", Encoding.GetEncoding("windows-1251"));
            while (!altText.EndOfStream)
            {
                string str = altText.ReadLine();
                rtbMiniText.AppendText(str + "\n");
            }
            altText.Close();

            altText = new StreamReader("files\\fullText.txt", Encoding.GetEncoding("windows-1251"));
            while (!altText.EndOfStream)
            {
                string str = altText.ReadLine();
                rtbFullText.AppendText(str + "\n");
            }
            altText.Close();

            altText = new StreamReader("files\\title.txt", Encoding.GetEncoding("windows-1251"));
            while (!altText.EndOfStream)
            {
                string str = altText.ReadLine();
                tbTitle.AppendText(str + "\n");
            }
            altText.Close();

            altText = new StreamReader("files\\description.txt", Encoding.GetEncoding("windows-1251"));
            while (!altText.EndOfStream)
            {
                string str = altText.ReadLine();
                tbDescription.AppendText(str + "\n");
            }
            altText.Close();

            altText = new StreamReader("files\\keywords.txt", Encoding.GetEncoding("windows-1251"));
            while (!altText.EndOfStream)
            {
                string str = altText.ReadLine();
                tbKeywords.AppendText(str + "\n");
            }
            altText.Close();
        }

        private void btnPrice_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.login = tbLogin.Text;
            Properties.Settings.Default.password = tbPasswords.Text;
            Properties.Settings.Default.Save();

            minitextTemplate = MinitextStr();
            fullTextTemplate = FulltextStr();
            keywordsTextTemplate = tbKeywords.Lines[0].ToString();
            titleTextTemplate = tbTitle.Lines[0].ToString();
            descriptionTextTemplate = tbDescription.Lines[0].ToString();

            fileUrls = "";
            ofdLoadPrice.ShowDialog();

            fileUrls = ofdLoadPrice.FileName.ToString();

            if (ofdLoadPrice.FileName == "openFileDialog1" || ofdLoadPrice.FileName == "")
            {
                MessageBox.Show("Ошибка при выборе файла", "Ошибка файла");
                return;
            }

            Thread tabl = new Thread(() => UpdatePrice());
            forms = tabl;
            forms.IsBackground = true;
            forms.Start();
        }

        private void UpdatePrice()
        {
            string l = tbLogin.Text;
            string p = tbPasswords.Text;
            CookieContainer cookie = nethouse.CookieNethouse(tbLogin.Text, tbPasswords.Text);
            if (cookie.Count == 1)
            {
                MessageBox.Show("Логин или пароль для сайта введены не верно", "Ошибка логина/пароля");
                return;
            }

            File.Delete("naSite.csv");
            CreateCSV(cookie);

        }

        private void CreateCSV(CookieContainer cookie)
        {
            List<string> newProduct = newList();
            string razdelCSV = "";
            string miniRazdelCSV = "";

            FileInfo file = new FileInfo(fileUrls);
            ExcelPackage p = new ExcelPackage(file);
            ExcelWorksheet w = p.Workbook.Worksheets[1];
            int q = w.Dimension.Rows;
            for (int i = 9; q > i; i++)
            {
                if (w.Cells[i, 3].Value == null && w.Cells[i, 2].Value == null)
                {
                    i++;
                    razdelCSV = (string)w.Cells[i, 2].Value;
                    razdelCSV = razdelCSV.Trim();
                }
                else if (w.Cells[i, 3].Value == null)
                {
                    miniRazdelCSV = (string)w.Cells[i, 2].Value;
                    miniRazdelCSV = razdelCSV.Trim();
                }
                else
                {
                    if (razdelCSV == "Авто" || razdelCSV == "Боковой прицеп" || razdelCSV == "Велосипед ЗиП" || razdelCSV == "Бензопила, мотокоса" || razdelCSV == "Зимние виды товаров" || razdelCSV == " Охота, Рыбалка, Туризм" || razdelCSV == "Сварочное оборудование" || razdelCSV == "Станки деревообрабатывающие" || razdelCSV == "Прочие товары (автохимия, зарядники, инструмент, литература, масла, наклейки)" || razdelCSV == "Мототехника, Снегоходы, Прицепы, Мотоблоки" || razdelCSV == "Мотоодежда, экипировка" || razdelCSV == "Шлемы" || razdelCSV == " ЛАМПЫ")
                        continue;
                    string article = (string)w.Cells[i, 3].Value;
                    string name = (string)w.Cells[i, 5].Value;

                    string price = ReturnPrice(name, article);
                }
            }
        }

        private string ReturnPrice(string name, string article)
        {
            string price = "";
            CookieContainer cookieWW = httprequest.webCookie("http://w-motors.ru/");
            string otv = getRequestEncod(cookieWW, "http://w-motors.ru/search/?q=" + name + "&amp;s=%CF%EE%E8%F1%EA", name);
            string urlTovarWW = new Regex("(?<=a href=\")/catalog[\\w\\W]*?(?=\">)").Match(otv).ToString();
            string nameTovarWW = new Regex("(?<=\">)<b>[\\w\\W]*?(?=</td>)").Match(otv).ToString();
            nameTovarWW = nameTovarWW.Replace("<b>", "").Replace("</b>", "").Replace("&quot;", "\"");
            if (name == nameTovarWW)
            {
                otv = httprequest.getRequestEncod("http://w-motors.ru" + urlTovarWW);
                string tovarCart = new Regex("<h1>[\\w\\W]*?(?=Заказ отсутствующих в каталоге товаров)").Match(otv).ToString();
                price = new Regex("(?<=\"><span>).*?(?=</span>)").Match(tovarCart).ToString();
            }
            else
            {

            }

            return price;
        }

        private void btnSaveTemplate_Click(object sender, EventArgs e)
        {
            int count = 0;
            StreamWriter writers = new StreamWriter("files\\miniText.txt", false, Encoding.GetEncoding(1251));
            count = rtbMiniText.Lines.Length;
            for (int i = 0; rtbMiniText.Lines.Length > i; i++)
            {
                if (count - 1 == i)
                {
                    if (rtbFullText.Lines[i] == "")
                        break;
                }
                writers.WriteLine(rtbMiniText.Lines[i].ToString());
            }
            writers.Close();

            writers = new StreamWriter("files\\fullText.txt", false, Encoding.GetEncoding(1251));
            count = rtbFullText.Lines.Length;
            for (int i = 0; count > i; i++)
            {
                if (count - 1 == i)
                {
                    if (rtbFullText.Lines[i] == "")
                        break;
                }
                writers.WriteLine(rtbFullText.Lines[i].ToString());
            }
            writers.Close();

            writers = new StreamWriter("files\\title.txt", false, Encoding.GetEncoding(1251));
            writers.WriteLine(tbTitle.Lines[0]);
            writers.Close();

            writers = new StreamWriter("files\\description.txt", false, Encoding.GetEncoding(1251));
            writers.WriteLine(tbDescription.Lines[0]);
            writers.Close();

            writers = new StreamWriter("files\\keywords.txt", false, Encoding.GetEncoding(1251));
            writers.WriteLine(tbKeywords.Lines[0]);
            writers.Close();

            MessageBox.Show("Сохранено");
        }

        private string MinitextStr()
        {
            string minitext = "";
            for (int z = 0; rtbMiniText.Lines.Length > z; z++)
            {
                if (rtbMiniText.Lines[z].ToString() == "")
                {
                    minitext += "<p><br /></p>";
                }
                else
                {
                    minitext += "<p>" + rtbMiniText.Lines[z].ToString() + "</p>";
                }
            }
            return minitext;
        }

        private string FulltextStr()
        {
            string fullText = "";
            for (int z = 0; rtbFullText.Lines.Length > z; z++)
            {
                if (rtbFullText.Lines[z].ToString() == "")
                {
                    fullText += "<p><br /></p>";
                }
                else
                {
                    fullText += "<p>" + rtbFullText.Lines[z].ToString() + "</p>";
                }
            }
            return fullText;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tbLogin.Text = Properties.Settings.Default.login;
            tbPasswords.Text = Properties.Settings.Default.password;
        }

        private List<string> newList()
        {
            List<string> newProduct = new List<string>();
            newProduct.Add("id");                                                                               //id
            newProduct.Add("Артикул *");                                                 //артикул
            newProduct.Add("Название товара *");                                          //название
            newProduct.Add("Стоимость товара *");                                    //стоимость
            newProduct.Add("Стоимость со скидкой");                                       //со скидкой
            newProduct.Add("Раздел товара *");                                         //раздел товара
            newProduct.Add("Товар в наличии *");                                                    //в наличии
            newProduct.Add("Поставка под заказ *");                                                 //поставка
            newProduct.Add("Срок поставки (дни) *");                                           //срок поставки
            newProduct.Add("Краткий текст");                                 //краткий текст
            newProduct.Add("Текст полностью");                                          //полностью текст
            newProduct.Add("Заголовок страницы (title)");                               //заголовок страницы
            newProduct.Add("Описание страницы (description)");                                 //описание
            newProduct.Add("Ключевые слова страницы (keywords)");                                 //ключевые слова
            newProduct.Add("ЧПУ страницы (slug)");                                   //ЧПУ
            newProduct.Add("С этим товаром покупают");                              //с этим товаром покупают
            newProduct.Add("Рекламные метки");
            newProduct.Add("Показывать на сайте *");                                           //показывать
            newProduct.Add("Удалить *");                                    //удалить
            files.fileWriterCSV(newProduct, "naSite");
            return newProduct;
        }

        public string getRequestEncod(CookieContainer cookie, string url, string name)
        {
            string otv = "";
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Accept = "*/*";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.CookieContainer = cookie;
            string request = "ajax_call=y&INPUT_ID=title-search-input&q=" + name;

            byte[] ms = System.Text.Encoding.GetEncoding("utf-8").GetBytes(request);
            req.ContentLength = ms.Length;
            Stream stre = req.GetRequestStream();
            stre.Write(ms, 0, ms.Length);
            stre.Close();
            HttpWebResponse res1 = (HttpWebResponse)req.GetResponse();
            StreamReader ressr1 = new StreamReader(res1.GetResponseStream(), Encoding.GetEncoding(1251));
            otv = ressr1.ReadToEnd();
            res1.Close();

            return otv;
        }
    }


}
