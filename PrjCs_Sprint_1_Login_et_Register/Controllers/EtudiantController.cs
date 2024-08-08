using PrjCs_Sprint_1_Login_et_Register.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Web.UI.WebControls;
using System.Web.Helpers;

namespace PrjCs_Sprint_1_Login_et_Register.Controllers
{
    public class EtudiantController : Controller
    {
        // GET: Etudiant
        public ActionResult Index()
        {
           
            return View();
        }
        public ActionResult Compte()
        {
            int ActualyEleve = Convert.ToInt32( Session["Eleve"]) ;
            var Eleve = RecupEtudiant(ActualyEleve);

            return View(Eleve);
        }
        public ActionResult Mes_Cours()
        {

            return View();
        }
        public ActionResult Cours()
        {
            int id_cour=Convert.ToInt32( Request.QueryString["id"] );
            var MyDb = new ElearningDataLinqDataContext("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Maham\\Desktop\\PrjCs__LINQ_Sprint1_2_3\\PrjCs_Sprint_1_Login_et_Register\\PrjCs_Sprint_1_Login_et_Register\\App_Data\\Bd_elearning.mdf;Integrated Security=True");


               var id_videoss = MyDb.cours_videos.ToList();

            //var id_videoss =    from v in MyDb.cours_videos where v.id_cours==id_cour select v;
            string m = "";
            foreach (var i in id_videoss)
            {
                m += i;

            }
            return View();
        }
        [HttpPost]
        public ActionResult Cours(int ID_Cours)
        {
            int id_cours = Convert.ToInt32(Request.QueryString["id"]);

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

            return View(new Etudiant { Nom="Entrez votre nom",Email="Entrez votre couriel",Prenom="Entrez votre Prenom"} );
        }

        [HttpPost]
        public ActionResult Register(Etudiant etudiant,int NiveauEducation) 
        {
            Random rnd = new Random(DateTime.Now.Second);
            string nom = etudiant.Nom;
            string email = etudiant.Email;
            String prenom = etudiant.Prenom;
            int niveauEd = NiveauEducation;
            String Username = nom + prenom;
            String password = nom + rnd.Next(1, 100);
            SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Maham\Desktop\PrjCs__LINQ_Sprint1_2_3\PrjCs_Sprint_1_Login_et_Register\PrjCs_Sprint_1_Login_et_Register\App_Data\Bd_elearning.mdf;Integrated Security=True");

            try
            {
                string sql = "INSERT INTO eleves (nom, prenom, email, id_niveau) VALUES (@nom, @prenom, @email,@id_niveau)";
                con.Open();

                SqlCommand cmd = new SqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@nom", nom);
                cmd.Parameters.AddWithValue("@prenom", prenom);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@id_niveau", niveauEd);
                int IsGood = cmd.ExecuteNonQuery();

                if (IsGood != 0)
                {
                    SqlCommand cmdideleve = new SqlCommand("Select * From eleves where email=@email And nom=@nom;", con);
                    cmdideleve.Parameters.AddWithValue("@email", email);
                    cmdideleve.Parameters.AddWithValue("@nom", nom);

                    using (SqlDataReader readerid = cmdideleve.ExecuteReader())
                    {
                        if (readerid.Read())
                        {
                            int id_eleves = Convert.ToInt32(readerid["id"]);
                            readerid.Close();

                            string sqllog = "INSERT INTO login (us, pw, id_eleves,statut) VALUES (@us, @pw, @id_eleves,@statut)";

                            SqlCommand cmdlog = new SqlCommand(sqllog, con);

                            cmdlog.Parameters.AddWithValue("@us", Username);
                            cmdlog.Parameters.AddWithValue("@pw", password);
                            cmdlog.Parameters.AddWithValue("@id_eleves", id_eleves);
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

        public ActionResult Login( )
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
                SqlCommand cmd = new SqlCommand("select us,pw,statut,id_eleves from login where us=@Username And pw=@Password;", con);
                cmd.Parameters.AddWithValue("@Username", Username);
                cmd.Parameters.AddWithValue("@Password", Password);


                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Boolean vrai = reader.Read();
                    if (vrai)
                    {
                        String Statut = Convert.ToString(reader["statut"]);
                        int id_eleves = Convert.ToInt32(reader["id_eleves"]);
                        Session["Eleve"]=id_eleves;
                        reader.Close();
                        if (Statut == "True")
                        {
                            ReloginSurcharge model = new ReloginSurcharge
                            {
                                Statut = Statut,
                                id_eleves = id_eleves, 
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


            ViewBag.Message =( "Quelque chose n'a pas marcher ! "+ ViewBag.errmsg);
            return View(new Logine { UserName = "Username  ", Password = " Password " });


        }

        
        public ActionResult ReLogin(ReloginSurcharge model)
        {
            string Username = model.Username;
            string Password = model.Password;
            string Statut = "False";
            int id_eleves = model.id_eleves;

            SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Maham\Desktop\PrjCs__LINQ_Sprint1_2_3\PrjCs_Sprint_1_Login_et_Register\PrjCs_Sprint_1_Login_et_Register\App_Data\Bd_elearning.mdf;Integrated Security=True");


            try
            {
                string sql = "UPDATE login SET us =@Username, pw =@Password, statut =@Statut WHERE id_eleves =@id_eleves";
                con.Open();
                using (SqlCommand cmdRelog = new SqlCommand(sql,con))
                {
                    cmdRelog.Parameters.AddWithValue("@Username", Username);
                    cmdRelog.Parameters.AddWithValue("@Password", Password);
                    cmdRelog.Parameters.AddWithValue("@Statut", Statut);
                    cmdRelog.Parameters.AddWithValue("@id_eleves", id_eleves);
                    
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















        ///Mes Fonctions


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
            List<NiveauEducation> lstNiveau = new List<NiveauEducation>();

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

        public Etudiant RecupEtudiant( int id_eleve )
        {
            int ID_Eleve = id_eleve;
            Etudiant etudiant = new Etudiant();
            SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Maham\Desktop\PrjCs__LINQ_Sprint1_2_3\PrjCs_Sprint_1_Login_et_Register\PrjCs_Sprint_1_Login_et_Register\App_Data\Bd_elearning.mdf;Integrated Security=True");


            try
            {
                SqlCommand cmd = new SqlCommand("select * from eleves where Id=@ID_Eleve;", con);
                cmd.Parameters.AddWithValue("@ID_Eleve", ID_Eleve);


                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Boolean vrai = reader.Read();
                    if (vrai)
                    {
                        etudiant.Nom = Convert.ToString(reader["nom"]);
                        etudiant.Prenom = Convert.ToString(reader["prenom"]);
                        etudiant.Email = Convert.ToString(reader["email"]);
                        etudiant.Niv = Convert.ToInt32(reader["id_niveau"]);
                        reader.Close();
                        return etudiant ;

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
            return etudiant;

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