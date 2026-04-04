using cinema.Models;

namespace cinema.Services
{
    public interface PaymentService 
    {
        public bool create(Payment payment);
        public dynamic findById(int id);
        public dynamic findAll();
        public dynamic findByEmail(string email, string? phone = null);
        public bool updateStatus(int id, bool status, string? transactionNo = null, int? paymentType = null);
        public Payment? findPaymentEntityById(int id);
        public Payment? findByTransactionNo(string transNo);
        public bool delete(int id);
    }
}
