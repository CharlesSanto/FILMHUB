namespace FILMHUB.ViewModel;

public class ProfileViewModel
{
    public ReviewsViewModel Reviews { get; set; }
    public List<FavoriteMovieViewModel> FavoriteMovie { get; set; }   
}