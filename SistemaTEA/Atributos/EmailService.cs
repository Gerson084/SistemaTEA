using MailKit.Net.Smtp;
using MimeKit;
namespace SistemaTEA.Atributos
{
    public class EmailService
    {
        public void EnviarCorreoBienvenida(string correoDestino, string contraseñaTemporal)
        {
            var mensaje = new MimeMessage();
            mensaje.From.Add(new MailboxAddress("Sistema de Diagnosticos", "gerson7803@gmail.com"));
            mensaje.To.Add(MailboxAddress.Parse(correoDestino));
            mensaje.Subject = "Bienvenido al Sistema de Diagnosticos para TEA";
            string cuerpoHtml = $@"
                <html>
                <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
                    <div style='max-width: 600px; margin: auto; background: white; padding: 30px; border-radius: 10px; box-shadow: 0 0 10px rgba(0,0,0,0.1);'>
                        <h2 style='color: #32ab6c;'>Sistema de Diagnosticos para TEA</h2>
                        <p>Hola,</p>
                        <p>Tu cuenta ha sido creada correctamente. A continuación, te proporcionamos los datos de acceso:</p>
                        <ul style='line-height: 1.8;'>
                            <li><strong>Correo:</strong> {correoDestino}</li>
                            <li><strong>Contraseña:</strong> {contraseñaTemporal}</li>
                        </ul>
                        <p style='margin-top: 20px;'>Se te enviará una notificación a tu correo cuando se te brinde acceso al sistema.</p>
                        <p>Si no solicitaste esta cuenta, por favor ignora este correo.</p>
                        <br />
                        <p style='color: #888;'>Atentamente,<br/>Equipo de Sistema TEA</p>
                    </div>
                </body>
                </html>
                ";
            mensaje.Body = new TextPart("html")
            {
                Text = cuerpoHtml
            };
            using (var smtp = new SmtpClient())
            {
                smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate("gerson7803@gmail.com", "rlsi csng hryr kqiy");
                smtp.Send(mensaje);
                smtp.Disconnect(true);
            }
        }

        public void EnviarCorreoConfirmacionAcceso(string correoDestino)
        {
            var mensaje = new MimeMessage();
            mensaje.From.Add(new MailboxAddress("Sistema de Diagnosticos", "gerson7803@gmail.com"));
            mensaje.To.Add(MailboxAddress.Parse(correoDestino));
            mensaje.Subject = "Acceso concedido - Sistema de Diagnosticos para TEA";
            string cuerpoHtml = $@"
                <html>
                <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
                    <div style='max-width: 600px; margin: auto; background: white; padding: 30px; border-radius: 10px; box-shadow: 0 0 10px rgba(0,0,0,0.1);'>
                        <h2 style='color: #32ab6c;'>Sistema de Diagnosticos para TEA</h2>
                        <p>Hola,</p>
                        <p>¡Tu cuenta ya ha sido activada! Ahora puedes acceder al sistema con tus credenciales.</p>
                        <p style='margin-top: 20px;'>Gracias por ser parte de Sistema TEA.</p>
                        <br />
                        <p style='color: #888;'>Atentamente,<br/>Equipo de Sistema TEA</p>
                    </div>
                </body>
                </html> 
                ";
            mensaje.Body = new TextPart("html")
            {
                Text = cuerpoHtml
            };
            using (var smtp = new SmtpClient())
            {
                smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate("gerson7803@gmail.com", "rlsi csng hryr kqiy");
                smtp.Send(mensaje);
                smtp.Disconnect(true);
            }
        }

        public void EnviarCorreoBienvenidaPsicologo(string correoDestino, string contraseñaTemporal)
        {
            var mensaje = new MimeMessage();
            mensaje.From.Add(new MailboxAddress("Sistema de Diagnosticos", "gerson7803@gmail.com"));
            mensaje.To.Add(MailboxAddress.Parse(correoDestino));
            mensaje.Subject = "Cuenta activada - Bienvenido al Sistema de Diagnosticos para TEA";
            string cuerpoHtml = $@"
                <html>
                <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
                    <div style='max-width: 600px; margin: auto; background: white; padding: 30px; border-radius: 10px; box-shadow: 0 0 10px rgba(0,0,0,0.1);'>
                        <h2 style='color: #32ab6c;'>Sistema de Diagnosticos para TEA</h2>
                        <p>Estimado/a profesional,</p>
                        <p>¡Bienvenido al equipo! Ha sido registrado exitosamente como psicólogo en nuestro sistema de evaluación para Trastornos del Espectro Autista (TEA).</p>
                        <p style='background-color: #d4edda; padding: 15px; border: 1px solid #c3e6cb; border-radius: 5px; color: #155724;'>
                            <strong>✅ Su cuenta está ACTIVA y lista para usar inmediatamente.</strong>
                        </p>
                        <div style='background-color: #f8f9fa; padding: 20px; border-radius: 5px; border-left: 4px solid #32ab6c; margin: 20px 0;'>
                            <h3 style='color: #32ab6c; margin-top: 0;'>Sus credenciales de acceso:</h3>
                            <ul style='line-height: 1.8; margin: 0;'>
                                <li><strong>Correo electrónico:</strong> {correoDestino}</li>
                                <li><strong>Contraseña:</strong> {contraseñaTemporal}</li>
                            </ul>
                        </div>
                        <p><strong>Como psicólogo certificado, ya puede acceder a:</strong></p>
                        <ul style='line-height: 1.8;'>
                            <li>Herramientas de evaluación ADOS-2 (Autism Diagnostic Observation Schedule)</li>
                            <li>Instrumentos ADI-R (Autism Diagnostic Interview-Revised)</li>
                            <li>Gestión de pacientes asignados</li>
                            <li>Generación de reportes profesionales</li>
                        </ul>
                        <p style='background-color: #fff3cd; padding: 15px; border: 1px solid #ffeaa7; border-radius: 5px; color: #856404;'>
                            <strong>Recomendación de seguridad:</strong> Cambie su contraseña en el primer acceso al sistema.
                        </p>
                        <p>Puede iniciar sesión inmediatamente en el sistema. Si tiene alguna consulta técnica o requiere capacitación adicional, no dude en contactar al administrador.</p>
                        <br />
                        <p style='color: #888;'>Cordialmente,<br/>Administración del Sistema<br/>Sistema de Diagnosticos para TEA</p>
                    </div>
                </body>
                </html>
                ";
            mensaje.Body = new TextPart("html")
            {
                Text = cuerpoHtml
            };
            using (var smtp = new SmtpClient())
            {
                smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate("gerson7803@gmail.com", "rlsi csng hryr kqiy");
                smtp.Send(mensaje);
                smtp.Disconnect(true);
            }
        }
    }
}