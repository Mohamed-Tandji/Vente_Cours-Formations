using PrjCs_Sprint_1_Login_et_Register.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Data.Linq;
using System.Runtime.Serialization.Formatters;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net;

namespace PrjCs_Sprint_1_Login_et_Register.Controllers
{
    public class EnseignantController : Controller
    {
        // GET: Enseignant   
        //   ElearningDataLinqDataContext -- le nom de linq
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Menu()
        {
           
            

            var Cours = RecupCours();
            ViewBag.ModelCours = Cours;


            return View();
            
        }
        public ActionResult Register()
        {

            var niveauxEducation = RecupNiveau();
            ViewBag.ModelNiveauEducation = niveauxEducation;


            return View(new Enseignant { Nom = "Entrez votre nom", Email = "Entrez votre couriel", Prenom = "Entrez votre Prenom" });
        }

        [HttpPost]
        public ActionResult Register(Enseignant enseignant, int NiveauEducation)
        {
            Random rnd = new Random(DateTime.Now.Second);
            string nom = enseignant.Nom;
            string email = enseignant.Email;
            String prenom = enseignant.Prenom;
            int niveauEd = NiveauEducation;
            String Username = nom + prenom;
            String password = nom + rnd.Next(1, 100);
            SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Maham\Desktop\PrjCs__LINQ_Sprint1_2_3\PrjCs_Sprint_1_Login_et_Register\PrjCs_Sprint_1_Login_et_Register\App_Data\Bd_elearning.mdf;Integrated Security=True");

            try
            {
                string sql = "INSERT INTO enseignants (nom, prenom, email, id_niveau) VALUES (@nom, @prenom, @email,@id_niveau)";
                con.Open();

                SqlCommand cmd = new SqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@nom", nom);
                cmd.Parameters.AddWithValue("@prenom", prenom);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@id_niveau", niveauEd);
                int IsGood = cmd.ExecuteNonQuery();

                if (IsGood != 0)
                {
                    SqlCommand cmdideleve = new SqlCommand("Select * From enseignants where email=@email And nom=@nom;", con);
                    cmdideleve.Parameters.AddWithValue("@email", email);
                    cmdideleve.Parameters.AddWithValue("@nom", nom);

                    using (SqlDataReader readerid = cmdideleve.ExecuteReader())
                    {
                        if (readerid.Read())
                        {
                            int id_enseignants = Convert.ToInt32(readerid["id"]);
                            readerid.Close();

                            string sqllog = "INSERT INTO loginProf (us, pw, id_enseignants,statut) VALUES (@us, @pw, @id_enseignants,@statut)";

                            SqlCommand cmdlog = new SqlCommand(sqllog, con);

                            cmdlog.Parameters.AddWithValue("@us", Username);
                            cmdlog.Parameters.AddWithValue("@pw", password);
                            cmdlog.Parameters.AddWithValue("@id_enseignants", id_enseignants);
                            cmdlog.Parameters.AddWithValue("@statut", "True");


                            Email mail = new Email { To = email, Subject = "Vos informations de login ", Message = "Votre username est: " + Username + " et votre password est:  " + password };

                            int IsGoodlog = cmdlog.ExecuteNonQuery();
                            if (IsGoodlog != 0)
                            {
                                if (SendMail(mail))
                                {
                                    ViewBag.msg = "le mail contenant vos infos de connexion a bien été envoyer";
                                    return View("Reponse");
                                }
                                else
                                {
                                    ViewBag.msg = "le mail contenant vos infos de connexion n'a pas pu etre envoyer";
                                    return View("Reponse");
                                }

                            }
                            else
                            {
                                ViewBag.msg = "Insertion dans login echouer";

                                return View("Reponse");
                            }

                        }
                        else
                        {
                            ViewBag.msg = "Insertion dans login echouer";

                            return View("Reponse");
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                ViewBag.errmsg = ex.Message;
            }
            finally
            {
                con.Close();

            }
            return View("view");
        }

        public ActionResult Login()
        {


            return View(new Logine { UserName = "Username  ", Password = " Password " });


        }

        [HttpPost]

        public ActionResult Login(Logine login)
        {
            String Username = login.UserName;
            String Password = login.Password;
            SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Maham\Desktop\PrjCs__LINQ_Sprint1_2_3\PrjCs_Sprint_1_Login_et_Register\PrjCs_Sprint_1_Login_et_Register\App_Data\Bd_elearning.mdf;Integrated Security=True");


            try
            {
                SqlCommand cmd = new SqlCommand("select us,pw,statut,id_enseignants from loginProf where us=@Username And pw=@Password;", con);
                cmd.Parameters.AddWithValue("@Username", Username);
                cmd.Parameters.AddWithValue("@Password", Password);


                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Boolean vrai = reader.Read();
                    if (vrai)
                    {
                        String Statut = Convert.ToString(reader["statut"]);
                        int id_enseignants = Convert.ToInt32(reader["id_enseignants"]);

                        reader.Close();
                        if (Statut == "True")
                        {
                            ReloginSurchargeTeacher model = new ReloginSurchargeTeacher
                            {
                                Statut = Statut,
                                id_enseignants = id_enseignants,
                                Username = Username,
                                Password = Password
                            };

                            return View("ReLogin", model);
                        }


                        var lstCours = RecupCours();
                        ViewBag.ModelCours = lstCours;
                        return View("Menu");

                    }

                }



            }
            catch (Exception ex)
            {
                ViewBag.errmsg = ex.Message;
            }
            finally
            {
                con.Close();

            }


            ViewBag.Message = ("Quelque chose n'a pas marcher ! " + ViewBag.errmsg);
            return View(new Logine { UserName = "Username  ", Password = " Password " });


        }

        public ActionResult ReLogin(ReloginSurchargeTeacher model)
        {
            string Username = model.Username;
            string Password = model.Password;
            string Statut = "False";
            int id_enseignants = model.id_enseignants;

            SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Maham\Desktop\PrjCs__LINQ_Sprint1_2_3\PrjCs_Sprint_1_Login_et_Register\PrjCs_Sprint_1_Login_et_Register\App_Data\Bd_elearning.mdf;Integrated Security=True");


            try
            {
                string sql = "UPDATE loginProf SET us =@Username, pw =@Password, statut =@Statut WHERE id_enseignants =@id_enseignants";
                con.Open();
                using (SqlCommand cmdRelog = new SqlCommand(sql, con))
                {
                    cmdRelog.Parameters.AddWithValue("@Username", Username);
                    cmdRelog.Parameters.AddWithValue("@Password", Password);
                    cmdRelog.Parameters.AddWithValue("@Statut", Statut);
                    cmdRelog.Parameters.AddWithValue("@id_enseignants", id_enseignants);

                    int IsGoodRelog = cmdRelog.ExecuteNonQuery();

                    if (IsGoodRelog > 0)
                    {
                        ViewBag.Message = "Mise à jour réussie !";
                        var lstCours = RecupCours();
                        ViewBag.ModelCours = lstCours;
                        return View("Menu");
                    }
                    else
                    {
                        ViewBag.Message = "Oups!! La mise à jour a echoue";
                        return View("Login");
                    }
                }




            }
            catch (Exception ex)
            {
                ViewBag.errmsg = ex.Message;
            }
            finally
            {
                con.Close();

            }



            var lstCourss = RecupCours();
            ViewBag.ModelCours = lstCourss;
            return View("Menu");

        }




        public ActionResult Ajout_Video()
        {
            var niveauxEducation = RecupNiveau();
            ViewBag.ModelNiveauEducation = niveauxEducation;

            var lstCourss = RecupCours();
            ViewBag.ModelCours = lstCourss;


            return View(new Videos { Titre = " Titre ", Description = "  Description", Url = "", Img = "" });
        }


        [HttpPost]
        public ActionResult Ajout_Video(Videos video,int Cours, HttpPostedFileBase fileUrl, HttpPostedFileBase fileImage)
        {
            if (fileUrl != null && fileUrl.ContentLength > 0 )
            {
                var path_Video = Path.Combine(Server.MapPath("~/Content/Videos"), fileUrl.FileName);
                fileUrl.SaveAs(path_Video);
                var path_Image = Path.Combine(Server.MapPath("~/Content/Images"), fileImage.FileName);
                fileUrl.SaveAs(path_Image);

                string Url_Video = "Content/Videos/" + fileUrl.FileName;
                string Url_Image = "Content/Images/" + fileImage.FileName;

                string Titre = video.Titre;
                string Description_video = video.Description;
                int Id_Cour = Cours;
                int Id_niveau;


                try
                {
                    var MyBase = new ElearningDataLinqDataContext("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Maham\\Desktop\\PrjCs__LINQ_Sprint1_2_3\\PrjCs_Sprint_1_Login_et_Register\\PrjCs_Sprint_1_Login_et_Register\\App_Data\\Bd_elearning.mdf;Integrated Security=True");
                    
                        var cours = MyBase.cours.FirstOrDefault(C => C.Id == Id_Cour);
                        Id_niveau = cours.id_niveau;

                        var newVideo = new videos
                        {
                            titre = Titre,
                            description = Description_video,
                            url = Url_Video,
                            img = Url_Image,
                            id_niveau = Id_niveau
                        };

                        MyBase.videos.InsertOnSubmit(newVideo);
                        MyBase.SubmitChanges();

                    var Actvideo = MyBase.videos.FirstOrDefault(v => v.titre == Titre);  
                    int Id_video= Actvideo.Id;

                    var nouvelleVideo = new cours_videos
                    {
                        id_cours = Id_Cour,
                        id_videos = Id_video
                    };

                    MyBase.cours_videos.InsertOnSubmit(nouvelleVideo);
                    MyBase.SubmitChanges();

                    ViewBag.msgb = "Video ajouter avec succès!!!";
                        var lstCours = RecupCours();
                        ViewBag.ModelCours = lstCours;
                        return View("Menu");
                    
                }
                catch (Exception ex)
                {
                    ViewBag.msg = "Oups, quelque chose s'est mal passé : " + ex.Message;
                    var lstCourss = RecupCours();
                    ViewBag.ModelCours = lstCourss;
                    return View("Menu");
                }


            }

            ViewBag.msg = "Oups, quelque chose s'est mal passé !!! " ;
            var lstCour = RecupCours();
            ViewBag.ModelCours = lstCour;
            return View("Menu");

        }
        public ActionResult Creation_Cours()
        {
            var niveauxEducation = RecupNiveau();
            ViewBag.ModelNiveauEducation = niveauxEducation;


            return View(new Cours { Titre = " Titre ", Num_Cours = "  Numero de cours", Description = "  Description",Prix=0.0 ,Url=""});

        }
        [HttpPost]
        public ActionResult Creation_Cours(Cours cours, int NiveauEducation, HttpPostedFileBase file)
        {

            var path = Path.Combine(Server.MapPath("~/Content/Images"), file.FileName);
            file.SaveAs(path);


            string Nom_cours = cours.Titre;
            string Num_cours = cours.Num_Cours;
            string Description_cours = cours.Description;
            double Prix_cours = cours.Prix;
            string Url_cours = "Content/Images/" + file.FileName;
            int Id_Niveau = NiveauEducation;

            SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Maham\Desktop\PrjCs__LINQ_Sprint1_2_3\PrjCs_Sprint_1_Login_et_Register\PrjCs_Sprint_1_Login_et_Register\App_Data\Bd_elearning.mdf;Integrated Security=True");

            try
            {
                string sql = "INSERT INTO cours (titre, noCours, description,prix,url, id_niveau) VALUES (@titre, @noCours,@description,@prix,@url, @id_niveau)";
                con.Open();

                SqlCommand cmd = new SqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@titre", Nom_cours);
                cmd.Parameters.AddWithValue("@noCours", Num_cours);
                cmd.Parameters.AddWithValue("@description", Description_cours);
                cmd.Parameters.AddWithValue("@prix", Prix_cours);
                cmd.Parameters.AddWithValue("@url", Url_cours);
                cmd.Parameters.AddWithValue("@id_niveau", Id_Niveau);
                int IsGood= cmd.ExecuteNonQuery();
                if (IsGood != 0)
                {

                    ViewBag.msgb = "Cours creer avec success!!!";
                    var lstCours = RecupCours();
                    ViewBag.ModelCours = lstCours;
                    return View("Menu");
                }
              
          
                    
           

            }
            catch (Exception ex)
            {
                ViewBag.errmsg = ex.Message;
            }
            finally { con.Close(); }

            ViewBag.msg = "Oups Quelque chose c'est mal passee !!!";
            var lstCourss = RecupCours();
            ViewBag.ModelCours = lstCourss;
            return View("Menu");

        }


        private bool SendMail(Email email)
        {

            string sender = System.Configuration.ConfigurationManager.AppSettings["mailSender"];
            string pw = System.Configuration.ConfigurationManager.AppSettings["mailPw"];

            try
            {
                SmtpClient smtpclient = new SmtpClient("smtp.office365.com", 587);
                smtpclient.Timeout = 3000;
                smtpclient.EnableSsl = true;
                smtpclient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpclient.UseDefaultCredentials = false;
                smtpclient.Credentials = new NetworkCredential(sender, pw);


                MailMessage mailMessage = new System.Net.Mail.MailMessage(sender, email.To, email.Subject, email.Message);
                mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                smtpclient.Send(mailMessage);

                return true;


            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public List<NiveauEducation> RecupNiveau()
        {
            List<NiveauEducation> lstNiveau= new List<NiveauEducation>();

            SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Maham\Desktop\PrjCs__LINQ_Sprint1_2_3\PrjCs_Sprint_1_Login_et_Register\PrjCs_Sprint_1_Login_et_Register\App_Data\Bd_elearning.mdf;Integrated Security=True");

            try
            {
                SqlCommand cmd = new SqlCommand("select * from niveau;", con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lstNiveau.Add(new NiveauEducation
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Symboles = reader["symboles"].ToString(),


                        });
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                con.Close();

            }

            return lstNiveau;
        }

       



        public List<Cours> RecupCours()
        {
            List<Cours> lstCours = new List<Cours>();

            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Maham\Desktop\PrjCs__LINQ_Sprint1_2_3\PrjCs_Sprint_1_Login_et_Register\PrjCs_Sprint_1_Login_et_Register\App_Data\Bd_elearning.mdf;Integrated Security=True";

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select * from cours;", con);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lstCours.Add(new Cours
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Titre = reader["titre"].ToString(),
                                Num_Cours = reader["noCours"].ToString(),
                                Description = reader["description"].ToString(),
                                Prix = Convert.ToDouble(reader["prix"]),
                                Url = reader["url"].ToString(),
                                Id_niveau = Convert.ToInt32(reader["id_niveau"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return lstCours;

        }



    }
}