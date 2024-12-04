using System.ComponentModel.DataAnnotations;

namespace BackEnd.ViewModels
{
    public class BlogPostEditViewModel
    {
        public string PostId { get; set; }


        [Required(AllowEmptyStrings = false)]
        public string Title { get; set; }


        [Required(AllowEmptyStrings = false)]
        public string Content { get; set; }

    }
}
