using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Drawing;
using XWolf;


namespace Umbrella
{
    class uHttp : HttpServerProcessor
    {
        static byte[] logo;

        static uHttp(){
            MemoryStream ms=new MemoryStream();
            Properties.Resources.logo.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Close();
            logo = ms.ToArray();
        }

        private void Header()
        {
            Write("<html><head><title>Umbrella</title>");
            Write("<style>body,td{font-family:Tahoma;font-size:14px}.bix{font-size:100px}.error{color:red}</style>");
            Write("</head><body background='bg'><table align='center'><tr><td><img src='logo'></td><td class='bix'>Umbrella<small><small><small><small><small><small>" + Program.version + "</td></tr>");
            Write("<tr height='3'><td colspan=2 bgcolor=black></td></tr></table><center>");
#if TRACE
            Write(" TRACE ");
#endif
#if DEBUG
            Write(" DEBUG ");
#endif
            Write("</center><p><table width='600px' align='center'><tr><td width='150px' valign='top'>");
            //Listado servicios
            Write("<table width='100%' height='100%' style='border-right:1px dotted #CCCCCC'><tr><td bgcolor='#EEEEEE'>Servicios:</td></tr>");
            foreach (uService serv in uServices.Services)
            {
                Write("<tr><td align='right'><a href='" + serv.spath + "'>" + serv.Name + "</td></tr>");
            }
            Write("</table></td><td>");
        }

        private void Foot()
        {
            Out.Write("</td></tr></table></body></html>");
        }

        override protected void Process(){
            try
            {
                if (Path.StartsWith("/service_"))
                {
                    doService(Path.Substring(1));
                    return;
                }
            }
            catch (Exception e)
            {
                doError(e);
                return;
            }
            switch (Path.ToLower())
            {
                case "/":
                    doHome();
                    break;
                case "/favicon.ico":
                    doFavicon();
                    break;
                case "/logo":
                    doLogo();
                    break;
                case "/bg":
                    doBack();
                    break;
                default:
                    Error404();
                    break;
            }
        }

        private void doHome()
        {
            Header();
            
            
           
            //misc
            Foot();
        }

        private void doService(string srid)
        {
            uService srv=uServices.ByPath(srid);
            if (srv == null)
                throw new Exception("No existe el servicio");
            if (srv.Acciona(this))
                doGo(Path);
            else
            {
                 srv.setPath(srid);
                 if (srv.rsubpath == "")
                 {
                     Header();
                     srv.Execute();
                     Foot();
                 }
                 else if (srv.rsubpathbin)
                     srv.Execute(srv.rsubpath);
                 else
                 {
                     Header();
                     srv.Execute(srv.rsubpath);
                     Foot();
                 }
            }
        }

        private void doError(Exception e)
        {
            Header();
            Write("<span class='error'>"+e.GetType().FullName+": "+e.Message+"</span><p><small><small>");
            Write(nl2br(e.StackTrace));
            Write("</small></small><hr><center><a href='/'>Indice</a>");
            Foot();
        }

        private void doFavicon()
        {
            Response.Headers[HttpResponseHeader.ContentType] = "image/vnd.microsoft.icon";
            MemoryStream ms=new MemoryStream();
            Properties.Resources.ico.Save(ms);
            Write(ms.ToArray());
            ms.Close();
        }

        private void doBack()
        {
            Response.Headers[HttpResponseHeader.ContentType] = "image/jpeg";
            Write(loadBG());
        }

        private void doLogo()
        {
            Response.Headers[HttpResponseHeader.ContentType] = "image/png";
            Write(logo);
        }

        private void doGo(string href)
        {
            Write("<script>document.location='" + href + "';</script>");
        }

        private byte[] loadBG()
        {
            MemoryStream ms = new MemoryStream();
            Properties.Resources.ubg.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            ms.Close();
            return ms.ToArray();
        }
    }
}
