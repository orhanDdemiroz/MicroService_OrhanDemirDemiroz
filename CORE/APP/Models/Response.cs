namespace CORE.APP.Models
{
    // query cevapları için base
    public abstract class Response
    {
        public virtual int Id { get; set; }

        protected Response()
        {
        }

        protected Response(int id)
        {
            Id = id;
        }
    }
}
