using FILMHUB.Models;

namespace FILMHUB.ViewModel;

public class MovieDetailsViewModels
{
    public Movie Movie { get; set; }
    public UserMovie UserMovie { get; set; }
    public string Trailer { get; set; }
    
    public bool HasUserMovie => UserMovie != null;
}