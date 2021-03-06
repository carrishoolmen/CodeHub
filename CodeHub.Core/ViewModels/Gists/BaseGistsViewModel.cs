using System;
using GitHubSharp.Models;
using ReactiveUI;
using Xamarin.Utilities.Core.ViewModels;

namespace CodeHub.Core.ViewModels.Gists
{
    public abstract class BaseGistsViewModel : BaseViewModel
    {
        protected readonly ReactiveList<GistModel> GistsCollection = new ReactiveList<GistModel>();

        public IReadOnlyReactiveList<GistItemViewModel> Gists { get; private set; }

        private string _searchKeyword;
        public string SearchKeyword
        {
            get { return _searchKeyword; }
            set { this.RaiseAndSetIfChanged(ref _searchKeyword, value); }
        }

        protected BaseGistsViewModel()
        {
            var gotoCommand = new Action<GistModel>(x =>
            {
                var vm = CreateViewModel<GistViewModel>();
                vm.Id = x.Id;
                vm.Gist = x;
                ShowViewModel(vm);
            });

            Gists = GistsCollection.CreateDerivedCollection(
                x => CreateGistItemViewModel(x, _ => gotoCommand(x)),
                x => x.Description.ContainsKeyword(SearchKeyword),
                signalReset: this.WhenAnyValue(x => x.SearchKeyword));
        }

        private static GistItemViewModel CreateGistItemViewModel(GistModel gist, Action<GistItemViewModel> action)
        {
            var owner = (gist.Owner == null) ? "Anonymous" : gist.Owner.Login;
            var description = string.IsNullOrEmpty(gist.Description) ? "Gist " + gist.Id : gist.Description;
            var imageUrl = (gist.Owner == null) ? null : gist.Owner.AvatarUrl;
            return new GistItemViewModel(owner, imageUrl, description, gist.UpdatedAt, action);
        }
    }
}

