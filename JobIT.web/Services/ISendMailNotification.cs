using System.Threading.Tasks;

namespace JobIT.web.Services
{
    public interface ISendMailNotification
    {
        Task SendMail(string recieverEmail, string messageBody, string Subject);
    }
}