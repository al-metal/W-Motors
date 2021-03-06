﻿using Bike18;
using OfficeOpenXml;
using RacerMotors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Формирование_ЧПУ;
using NehouseLibrary;
using xNet;
using xNet.Net;

namespace W_Motors
{
    public partial class Form1 : Form
    {
        Thread forms;

        int numberStart;

        NehouseLibrary.nethouse nethouse = new NehouseLibrary.nethouse();
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
        CookieContainer cookieWW;

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
            CookieDictionary cookie = nethouse.CookieNethouse(tbLogin.Text, tbPasswords.Text);
            if (cookie.Count == 1)
            {
                MessageBox.Show("Логин или пароль для сайта введены не верно", "Ошибка логина/пароля");
                return;
            }

            File.Delete("naSite.csv");
            CreateCSV(cookie);

        }

        private void CreateCSV(CookieDictionary cookie)
        {
            ControlsFormEnabledFalse();

            File.Delete("naSite.csv");
            List<string> newProduct = newList();
            razdelCSV = "";
            string miniRazdelCSV = "";

            int countTovars = 0;

            FileInfo file2 = new FileInfo("Каталог.xlsx");
            ExcelPackage p2 = new ExcelPackage(file2);
            ExcelWorksheet w2 = p2.Workbook.Worksheets[1];
            int q2 = w2.Dimension.Rows;

            FileInfo file = new FileInfo(fileUrls);
            ExcelPackage p = new ExcelPackage(file);
            ExcelWorksheet w = p.Workbook.Worksheets[1];
            int q = w.Dimension.Rows;
            cookieWW = httprequest.webCookie("http://w-motors.ru/");
            for (int i = numberStart; q > i; i++)
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
                    if (razdelCSV == "Авто" || razdelCSV == "Боковой прицеп" || razdelCSV == "Велосипед ЗиП" || razdelCSV == "Бензопила, мотокоса" || razdelCSV == "Зимние виды товаров" || razdelCSV == "Охота, Рыбалка, Туризм" || razdelCSV == "Сварочное оборудование" || razdelCSV == "Станки деревообрабатывающие" || razdelCSV == "Прочие товары (автохимия, зарядники, инструмент, литература, масла, наклейки)" || razdelCSV == "Мототехника, Снегоходы, Прицепы, Мотоблоки" || razdelCSV == "Мотоодежда, экипировка" || razdelCSV == "Шлемы" || razdelCSV == "ЛАМПЫ"
                        || razdelCSV == "1 ТЕХНИКА: Мотоциклы, Снегоходы, Прицепы, Мотоблоки, Снегоуборщики"
                        || razdelCSV == "Бензо Генераторы, Мотопомпы, Компрессоры, Насосные станции"
                        || razdelCSV == "Бензопила, мотокоса"
                        || razdelCSV == "Велосипеды в сборе,  велоЗиП"
                        || razdelCSV == "Зимние виды товаров"
                        || razdelCSV == "Мотоодежда, экипировка"
                        || razdelCSV == "Охота, Рыбалка, Туризм, Одежда, Обувь"
                        || razdelCSV == "Сварочное оборудование"
                        || razdelCSV == "Станки деревообрабатывающие")
                        continue;


                    string article = (string)w.Cells[i, 3].Value;
                    string name = (string)w.Cells[i, 5].Value;
                    double price = (double)w.Cells[i, 10].Value;
                    price = Math.Round(price + (price * 0.05));
                    article = "WM_" + article;

                    for (int j = 2; q2 > j; j++)
                    {
                        string articlePriceB18 = "";
                        try
                        {
                            articlePriceB18 = (string)w2.Cells[j, 2].Value.ToString();
                        }
                        catch
                        {
                            continue;
                        }
                        if (articlePriceB18 == article)
                        {
                            double pricePriceB18 = (double)w2.Cells[j, 4].Value;
                            if (pricePriceB18 != price)
                            {
                                string id = (string)w2.Cells[j, 1].Value.ToString();
                                string namePriceB18 = (string)w2.Cells[j, 3].Value.ToString();
                                string priceSales = "";
                                string tovarsPriceB18 = "";
                                string reklamaPriceB18 = "";
                                try
                                {
                                    priceSales = (string)w2.Cells[j, 5].Value.ToString();
                                    tovarsPriceB18 = (string)w2.Cells[j, 16].Value.ToString();
                                    reklamaPriceB18 = (string)w2.Cells[j, 2].Value.ToString();
                                }
                                catch
                                {

                                }
                                string razdelPriceB18 = (string)w2.Cells[j, 6].Value.ToString();
                                string nalichiePriceB18 = (string)w2.Cells[j, 7].Value.ToString();
                                string postavkaPriceB18 = (string)w2.Cells[j, 8].Value.ToString();
                                string srokPriceB18 = (string)w2.Cells[j, 9].Value.ToString();
                                string miniTextPriceB18 = (string)w2.Cells[j, 10].Value.ToString();
                                string fultextPriceB18 = (string)w2.Cells[j, 11].Value.ToString();
                                string titlePriceB18 = (string)w2.Cells[j, 12].Value.ToString();
                                string descriptionPriceB18 = (string)w2.Cells[j, 13].Value.ToString();
                                string keywordsPriceB18 = (string)w2.Cells[j, 14].Value.ToString();
                                string slugPriceB18 = (string)w2.Cells[j, 15].Value.ToString();
                                
                                newProduct = new List<string>();
                                newProduct.Add("\"" + id + "\""); //id
                                newProduct.Add("\"" + articlePriceB18 + "\""); //артикул
                                newProduct.Add("\"" + namePriceB18 + "\"");  //название
                                newProduct.Add("\"" + price.ToString() + "\""); //стоимость
                                newProduct.Add("\"" + priceSales + "\""); //со скидкой
                                newProduct.Add("\"" + razdelPriceB18 + "\""); //раздел товара
                                newProduct.Add("\"" + "100" + "\""); //в наличии
                                newProduct.Add("\"" + "0" + "\"");//поставка
                                newProduct.Add("\"" + "1" + "\"");//срок поставки
                                newProduct.Add("\"" + miniTextPriceB18.Replace("\"", "\"\"") + "\"");//краткий текст
                                newProduct.Add("\"" + fultextPriceB18.Replace("\"", "\"\"") + "\"");//полностью текст
                                newProduct.Add("\"" + titlePriceB18.Replace("\"", "\"\"") + "\""); //заголовок страницы
                                newProduct.Add("\"" + descriptionPriceB18.Replace("\"", "\"\"") + "\""); //описание
                                newProduct.Add("\"" + keywordsPriceB18.Replace("\"", "\"\"") + "\"");//ключевые слова
                                newProduct.Add("\"" + slugPriceB18 + "\""); //ЧПУ
                                newProduct.Add("\"" + tovarsPriceB18 + "\""); //с этим товаром покупают
                                newProduct.Add("\"" + reklamaPriceB18 + "\"");   //рекламные метки
                                newProduct.Add("\"" + "1" + "\"");  //показывать
                                newProduct.Add("\"" + "0" + "\""); //удалить
                                files.fileWriterCSV(newProduct, "naSite");
                            }
                        }

                    }


                    /*List<string> tovarWW = GetTovarWW(article, name, price);

                    string resultSearch = SearchInBike18(tovarWW);
                    if (resultSearch == null || resultSearch == "")
                    {
                        //WriteTovarInCSV(tovarWW);
                        countTovars++;
                    }
                    else
                    {
                        UpdatePrice(cookie, resultSearch, tovarWW);
                        //обновить цену
                    }*/
                }
                /*if (countTovars == 40000)
                {
                    cookie = nethouse.CookieNethouse(tbLogin.Text, tbPasswords.Text);
                    UploadCSVInNethoise(cookie);
                    countTovars = 0;
                    File.Delete("naSite.csv");
                    newProduct = newList();
                }*/

                SaveNumberStart(i);
            }

            SaveNumberStart(9);

            cookie = nethouse.CookieNethouse(tbLogin.Text, tbPasswords.Text);
            UploadCSVInNethoise(cookie);

            ControlsFormEnabledTrue();
        }

        private static void SaveNumberStart(int number)
        {
            Properties.Settings.Default.numberStart = number;
            Properties.Settings.Default.Save();
        }

        private void UpdatePrice(CookieDictionary cookie, string resultSearch, List<string> tovarWW)
        {
            string[] products = resultSearch.Split(';');
            foreach (string ss in products)
            {
                if (ss == "")
                    continue;
                List<string> listProduct = nethouse.GetProductList(cookie, ss);
                if (listProduct == null)
                    continue;
                listProduct[9] = tovarWW[2];
                nethouse.SaveTovar(cookie, listProduct);
            }
        }

        private void UploadCSVInNethoise(CookieDictionary cookie)
        {
            /*string[] naSite1 = File.ReadAllLines("naSite.csv", Encoding.GetEncoding(1251));
            if (naSite1.Length > 1)
                nethouse.(cookie, "naSite.csv");*/
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

        private List<string> GetTovarWW(string article, string name, double priceWW)
        {
            List<string> tovarWW = new List<string>();


            string price = "";
            string razdel = "";
            string slug = "";
            string titleText = "";
            string descriptionText = "";
            string keywordsText = "";
            string miniText = "";
            string fullText = "";

            string otv = getRequestEncod(cookieWW, "http://w-motors.ru/search/?q=" + name + "&amp;s=%CF%EE%E8%F1%EA", name);
            if (otv != "err")
            {
                string urlTovarWW = new Regex("(?<=a href=\")/catalog[\\w\\W]*?(?=\">)").Match(otv).ToString();
                string nameTovarWW = new Regex("(?<=\">)<b>[\\w\\W]*?(?=</td>)").Match(otv).ToString();
                nameTovarWW = nameTovarWW.Replace("<b>", "").Replace("</b>", "").Replace("&quot;", "\"");
                if (name == nameTovarWW)
                {
                    otv = nethouse.getRequestEncoding1251("http://w-motors.ru" + urlTovarWW);
                }
                else
                {
                    otv = nethouse.getRequestEncoding1251("http://w-motors.ru" + urlTovarWW);
                }

                name = name.Replace("\"", "").Replace("\r", "").Replace("\n", "").Replace("/", "_");

                miniText = minitextTemplate;
                fullText = fullTextTemplate;
                razdel = ReturnRazdel();

                descriptionTovarWW = "";
                descriptionTovarWW = new Regex("(?<=<div class=\"product-detail-text\">)[\\w\\W]*?(?=</div>)").Match(otv).ToString();
                MatchCollection ampersant = new Regex("&.*?;").Matches(descriptionTovarWW);
                foreach (Match str in ampersant)
                {
                    string s = str.ToString();
                    descriptionTovarWW = descriptionTovarWW.Replace(s, "").Replace("\"", "");
                }
                descriptionTovarWW = descriptionTovarWW.Replace("\n", "").Replace("\t", "");

                price = ReturnPrice(name, article, otv);

                if (price == "")
                {
                    price = priceWW.ToString();
                }

                string oldArticle = article;
                article = "WM_" + article;
                article = ReturnArticle(article);

                slug = chpu.vozvr(name);

                descriptionText = descriptionTextTemplate;
                titleText = titleTextTemplate;
                keywordsText = keywordsTextTemplate;

                titleText = ReplaceSEO("title", titleText, name, oldArticle.Replace(";", " "), article.Replace(";", " "));
                descriptionText = ReplaceSEO("description", descriptionText, name, oldArticle, article);
                keywordsText = ReplaceSEO("keywords", keywordsText, name, oldArticle, article);

                miniText = Replace(miniText, name, article);
                miniText = miniText.Remove(miniText.LastIndexOf("<p>"));

                fullText = Replace(fullText, name, article);
                fullText = fullText.Remove(fullText.LastIndexOf("<p>"));
                if (descriptionTovarWW != "\n\t" && descriptionTovarWW != "" && !descriptionTovarWW.Contains("https://") && !descriptionTovarWW.Contains("http://"))
                    fullText = "<p>" + descriptionTovarWW + "</p><p></p>" + fullText;
            }

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
            article = article.Replace("!", "_").Replace("@", "_").Replace("#", "_").Replace("$", "_").Replace("%", "_").Replace("^", "_").Replace("&", "_").Replace("*", "_").Replace("(", "_").Replace(")", "_").Replace("-", "_").Replace("+", "_").Replace("=", "_").Replace("/", "_").Replace("\\", "_").Replace("---", "_").Replace("___", "_").Replace("__", "_").Replace("|", "");

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

            numberStart = Properties.Settings.Default.numberStart;

            if (numberStart == 0)
            {
                numberStart = 9;
            }
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
            try
            {

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
            }
            catch
            {
                otv = "err";
            }

            return otv;
        }

        private string ReplaceSEO(string nameSEO, string text, string nameTovar, string oldArticle, string article)
        {
            text = text.Replace("НАЗВАНИЕ", nameTovar).Replace("АРТИКУЛ", oldArticle + ";" + article);

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
                if (text.Contains(" "))
                    text = text.Remove(text.LastIndexOf(" "));
                else
                    text = text.Remove(text.LastIndexOf(" "));
            }
            return text;
        }

        private void ControlsFormEnabledFalse()
        {
            btnPrice.Invoke(new Action(() => btnPrice.Enabled = false));
            btnImages.Invoke(new Action(() => btnImages.Enabled = false));
            btnSaveTemplate.Invoke(new Action(() => btnSaveTemplate.Enabled = false));
            rtbFullText.Invoke(new Action(() => rtbFullText.Enabled = false));
            rtbMiniText.Invoke(new Action(() => rtbMiniText.Enabled = false));
            tbDescription.Invoke(new Action(() => tbDescription.Enabled = false));
            tbKeywords.Invoke(new Action(() => tbKeywords.Enabled = false));
            tbTitle.Invoke(new Action(() => tbTitle.Enabled = false));
            tbLogin.Invoke(new Action(() => tbLogin.Enabled = false));
            tbPasswords.Invoke(new Action(() => tbPasswords.Enabled = false));
        }

        private void ControlsFormEnabledTrue()
        {
            btnPrice.Invoke(new Action(() => btnPrice.Enabled = true));
            btnImages.Invoke(new Action(() => btnImages.Enabled = true));
            btnSaveTemplate.Invoke(new Action(() => btnSaveTemplate.Enabled = true));
            rtbFullText.Invoke(new Action(() => rtbFullText.Enabled = true));
            rtbMiniText.Invoke(new Action(() => rtbMiniText.Enabled = true));
            tbDescription.Invoke(new Action(() => tbDescription.Enabled = true));
            tbKeywords.Invoke(new Action(() => tbKeywords.Enabled = true));
            tbTitle.Invoke(new Action(() => tbTitle.Enabled = true));
            tbLogin.Invoke(new Action(() => tbLogin.Enabled = false));
            tbPasswords.Invoke(new Action(() => tbPasswords.Enabled = false));
        }
    }


}
