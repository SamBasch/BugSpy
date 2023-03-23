using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugSpy.Models.ViewModels
{
    public class TicketEditViewModel
    {

        public Ticket? Ticket { get; set; }

        public SelectList? DevList { get; set; }    

        public SelectList? StatusList { get; set; }

        public SelectList? PriorityList { get; set; }

        public SelectList? TypeList { get; set; }   

        public string? DevId { get; set; }  

        public int? TicketType { get; set; }    

        public int? TicketStatus { get; set; }

        public int? TicketPriority { get; set; }    



    }
}
