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
using Формирование_ЧПУ;

namespace W_Motors
{
    public partial class Form1 : Form
    {
        Thread forms;

        nethouse nethouse = new nethouse();
        WebClient webClient = new WebClient();
        httpRequest httprequest = new httpRequest();
        FileEdit files = new FileEdit();
        CHPU chpu = new CHPU();

        string minitextTemplate;
        string fullTextTemplate;
        string keywordsTextTemplate;
        string titleTextTemplate;
        string descriptionTextTemplate;
        string razdelCSV = "";
        string boldOpen = "<span style=\"\"font-weight: bold; font-weight: bold; \"\">";
        string boldClose = "</span>";

        List<string> newProduct = new List<string>();

        string fileUrls;
        string descriptionTovarWW;

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
            razdelCSV = "";
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
                    if (razdelCSV == "Авто" || razdelCSV == "Боковой прицеп" || razdelCSV == "Велосипед ЗиП" || razdelCSV == "Бензопила, мотокоса" || razdelCSV == "Зимние виды товаров" || razdelCSV == "Охота, Рыбалка, Туризм" || razdelCSV == "Сварочное оборудование" || razdelCSV == "Станки деревообрабатывающие" || razdelCSV == "Прочие товары (автохимия, зарядники, инструмент, литература, масла, наклейки)" || razdelCSV == "Мототехника, Снегоходы, Прицепы, Мотоблоки" || razdelCSV == "Мотоодежда, экипировка" || razdelCSV == "Шлемы" || razdelCSV == "ЛАМПЫ")
                        continue;
                    
                    string article = (string)w.Cells[i, 3].Value;
                    string name = (string)w.Cells[i, 5].Value;

                    List<string> tovarWW = GetTovarWW(article, name);

                    string resultSearch = SearchInBike18(tovarWW);
                    if (resultSearch == null || resultSearch == "")
                    {
                        WriteTovarInCSV(tovarWW);
                    }
                    else
                    {
                        //обновить цену
                    }
                }
            }
        }

        private void WriteTovarInCSV(List<string> tovarMotoPiter)
        {
            string nameTovar = tovarMotoPiter[0].ToString();
            string article = tovarMotoPiter[1].ToString();
            string price = tovarMotoPiter[2].ToString();
            string categoryTovar = tovarMotoPiter[3].ToString();
            string slug = tovarMotoPiter[4].ToString();
            string titleText = tovarMotoPiter[5].ToString();
            string descriptionText = tovarMotoPiter[6].ToString();
            string keywordsText = tovarMotoPiter[7].ToString();
            string minitext = tovarMotoPiter[8].ToString();
            string fullText = tovarMotoPiter[9].ToString();

            newProduct = new List<string>();
            newProduct.Add(""); //id
            newProduct.Add("\"" + article + "\""); //артикул
            newProduct.Add("\"" + nameTovar + "\"");  //название
            newProduct.Add("\"" + price + "\""); //стоимость
            newProduct.Add("\"" + "" + "\""); //со скидкой
            newProduct.Add("\"" + categoryTovar + "\""); //раздел товара
            newProduct.Add("\"" + "100" + "\""); //в наличии
            newProduct.Add("\"" + "0" + "\"");//поставка
            newProduct.Add("\"" + "1" + "\"");//срок поставки
            newProduct.Add("\"" + minitext + "\"");//краткий текст
            newProduct.Add("\"" + fullText + "\"");//полностью текст
            newProduct.Add("\"" + titleText + "\""); //заголовок страницы
            newProduct.Add("\"" + descriptionText + "\""); //описание
            newProduct.Add("\"" + keywordsText + "\"");//ключевые слова
            newProduct.Add("\"" + slug + "\""); //ЧПУ
            newProduct.Add(""); //с этим товаром покупают
            newProduct.Add("");   //рекламные метки
            newProduct.Add("\"" + "1" + "\"");  //показывать
            newProduct.Add("\"" + "0" + "\""); //удалить

            files.fileWriterCSV(newProduct, "naSite");
        }

        private List<string> GetTovarWW(string article, string name)
        {
            List<string> tovarWW = new List<string>();

            CookieContainer cookieWW = httprequest.webCookie("http://w-motors.ru/");
            string otv = getRequestEncod(cookieWW, "http://w-motors.ru/search/?q=" + name + "&amp;s=%CF%EE%E8%F1%EA", name);
            string urlTovarWW = new Regex("(?<=a href=\")/catalog[\\w\\W]*?(?=\">)").Match(otv).ToString();
            string nameTovarWW = new Regex("(?<=\">)<b>[\\w\\W]*?(?=</td>)").Match(otv).ToString();
            nameTovarWW = nameTovarWW.Replace("<b>", "").Replace("</b>", "").Replace("&quot;", "\"");
            if (name == nameTovarWW)
            {
                otv = httprequest.getRequestEncod("http://w-motors.ru" + urlTovarWW);
            }
            else
            {
                otv = httprequest.getRequestEncod("http://w-motors.ru" + urlTovarWW);
            }

                string razdel = "";
            string miniText = minitextTemplate;
            string fullText = fullTextTemplate;
            razdel = ReturnRazdel();

            descriptionTovarWW = "";
            descriptionTovarWW = new Regex("(?<=<div class=\"product-detail-text\">)[\\w\\W]*?(?=</div>)").Match(otv).ToString();
            MatchCollection ampersant = new Regex("&.*?;").Matches(descriptionTovarWW);
            foreach(Match str in ampersant)
            {
                string s = str.ToString();
                descriptionTovarWW = descriptionTovarWW.Replace(s, "");
            }
            
            string price = ReturnPrice(name, article, otv);

            article = "WM_" + article;
            article = ReturnArticle(article);

            string slug = chpu.vozvr(name);

            string descriptionText = descriptionTextTemplate;
            string titleText = titleTextTemplate;
            string keywordsText = keywordsTextTemplate;

            titleText = ReplaceSEO("title", titleText, name, article.Replace(";", " "));
            descriptionText = ReplaceSEO("description", descriptionText, name, article);
            keywordsText = ReplaceSEO("keywords", keywordsText, name, article);

            miniText = Replace(miniText, name, article);
            miniText = miniText.Remove(miniText.LastIndexOf("<p>"));

            fullText = Replace(fullText, name, article);
            fullText = fullText.Remove(fullText.LastIndexOf("<p>"));
            fullText = "<p>" + descriptionTovarWW + "</p><p></p>" + fullText;

            tovarWW.Add(name);
            tovarWW.Add(article);
            tovarWW.Add(price);
            tovarWW.Add(razdel);
            tovarWW.Add(slug);
            tovarWW.Add(titleText);
            tovarWW.Add(descriptionText);
            tovarWW.Add(keywordsText);
            tovarWW.Add(miniText);
            tovarWW.Add(fullText);

            return tovarWW;
        }

        private string SearchInBike18(List<string> tovarWW)
        {
            string urlTovar = "";

            string nameTovar = tovarWW[0].ToString();
            string articles = tovarWW[1].ToString();
            string[] article = articles.Split(';');

            foreach (string str in article)
            {
                string search = "";
                if (urlTovar == "" || urlTovar == null)
                {
                    search = nethouse.searchTovar(nameTovar, str);
                    if (search != null)
                    {
                        urlTovar = urlTovar + ";" + search;
                    }
                }
            }

            return urlTovar;
        }

        private string Replace(string text, string nameTovar, string article)
        {
            string discount = Discount();
            string nameText = boldOpen + nameTovar + boldClose;
            text = text.Replace("СКИДКА", discount).Replace("НАЗВАНИЕ", nameText).Replace("АРТИКУЛ", article).Replace("<p><br /></p><p><br /></p><p><br /></p><p>", "<p><br /></p>");
            return text;
        }

        private string Discount()
        {
            string discount = "<p style=\"\"text-align: right;\"\"><span style=\"\"font -weight: bold; font-weight: bold;\"\"> Сделай ТРОЙНОЙ удар по нашим ценам! </span></p><p style=\"\"text-align: right;\"\"><span style=\"\"font -weight: bold; font-weight: bold;\"\"> 1. <a target=\"\"_blank\"\" href =\"\"http://bike18.ru/stock\"\"> Скидки за отзывы о товарах!</a> </span></p><p style=\"\"text-align: right;\"\"><span style=\"\"font -weight: bold; font-weight: bold;\"\"> 2. <a target=\"\"_blank\"\" href =\"\"http://bike18.ru/stock\"\"> Друзьям скидки и подарки!</a> </span></p><p style=\"\"text-align: right;\"\"><span style=\"\"font -weight: bold; font-weight: bold;\"\"> 3. <a target=\"\"_blank\"\" href =\"\"http://bike18.ru/stock\"\"> Нашли дешевле!? 110% разницы Ваши!</a></span></p>";
            return discount;
        }

        private string ReturnArticle(string article)
        {
            article = article.Replace("!", "_").Replace("@", "_").Replace("#", "_").Replace("$", "_").Replace("%", "_").Replace("^", "_").Replace("&", "_").Replace("*", "_").Replace("(", "_").Replace(")", "_").Replace("-", "_").Replace("+", "_").Replace("=", "_").Replace("/", "_").Replace("\\", "_").Replace("---", "_").Replace("___", "_").Replace("__", "_");

            return article;
        }

        private string ReturnRazdel()
        {
            string categoryName = "";

            switch (razdelCSV)
            {
                case "Бензо Генераторы, Мотопомпы":
                    categoryName = "Бензогенераторы, Мотопомпы";
                    break;
                case "Двигатели LIFAN (Запчасти)":
                    categoryName = "Двигатели";
                    break;
                case "Двигатели в сборе универсальные (LIFAN, BASHAN, CHAMPION)":
                    categoryName = "Двигатели";
                    break;
                case "Лодки":
                    categoryName = "Лодки и лодочные двигатели";
                    break;
                case "Лодочные двигатели":
                    categoryName = "Лодки и лодочные двигатели";
                    break;
                case "Мотокультиваторы, Мотоблоки, Минитракторы  ЗИП":
                    categoryName = "Запчасти мотокультиваторы, мотоблоки, минитрактора";
                    break;
                case "Подшипники":
                    categoryName = "Подшипники";
                    break;
                case "СВЕЧИ":
                    categoryName = "Свечи";
                    break;
                case "Скутера, Мопеды, Мотоциклы, ATV (импорт) ЗиП":
                    categoryName = "мопеды и мотоциклы китайского п-ва";
                    break;
                case "Снегоуборочные машины":
                    categoryName = "Снегоуборочные машины";
                    break;
                case "Снегоход Буран":
                    categoryName = "Отечественные снегоходы";
                    break;
                case "Снегоход Рысь":
                    categoryName = "Отечественные снегоходы";
                    break;
                case "Снегоход Тайга":
                    categoryName = "Отечественные снегоходы";
                    break;
                case "Урал":
                    categoryName = "Днепр, МТ, урал";
                    break;
                case "Днепр, МТ":
                    categoryName = "Днепр, МТ, урал";
                    break;
                case "Ява":
                    categoryName = "Запчасти и расходники для отечественной (советской) мототехники";
                    break;
                case "Муравей, Тула":
                    categoryName = "Запчасти и расходники для отечественной (советской) мототехники";
                    break;
                case "Минск":
                    categoryName = "Запчасти и расходники для отечественной (советской) мототехники";
                    break;
                case "Минск, Восход, Сова - ЗиП":
                    categoryName = "Запчасти и расходники для отечественной (советской) мототехники";
                    break;
                case "Мопед, Веломотор, ЗиД50":
                    categoryName = "Запчасти и расходники для отечественной (советской) мототехники";
                    break;
                case "Иж Планета":
                    categoryName = "Запчасти и расходники для отечественной (советской) мототехники";
                    break;
                case "Иж Юпитер":
                    categoryName = "Запчасти и расходники для отечественной (советской) мототехники";
                    break;
                case "Иж Юпитер, Иж Планета":
                    categoryName = "Запчасти и расходники для отечественной (советской) мототехники";
                    break;
                case "Восход, Сова":
                    categoryName = "Запчасти и расходники для отечественной (советской) мототехники";
                    break;
                default:
                    break;
            }

            string category = "Запчасти и расходники => Каталог запчастей и расходников BIKE18.RU => " + categoryName;

            return category;
        }

        private string ReturnPrice(string name, string article, string otv)
        {
            string price = "";
            
                string tovarCart = new Regex("<h1>[\\w\\W]*?(?=Заказ отсутствующих в каталоге товаров)").Match(otv).ToString();
                price = new Regex("(?<=\"><span>).*?(?=</span>)").Match(tovarCart).ToString();
            price = price.Replace(" ", "");

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

        private string ReplaceSEO(string nameSEO, string text, string nameTovar, string article)
        {
            text = text.Replace("НАЗВАНИЕ", nameTovar).Replace("АРТИКУЛ", article);

            switch (nameSEO)
            {
                case "title":
                    text = RemoveText(text, 255);
                    break;
                case "description":
                    text = RemoveText(text, 200);
                    break;
                case "keywords":
                    text = RemoveText(text, 100);
                    break;
                default:
                    text = RemoveText(text, 100);
                    break;
            }

            return text;
        }

        private string RemoveText(string text, int v)
        {
            if (text.Length > v)
            {
                text = text.Remove(v);
                text = text.Remove(text.LastIndexOf(" "));
            }
            return text;
        }
    }


}
