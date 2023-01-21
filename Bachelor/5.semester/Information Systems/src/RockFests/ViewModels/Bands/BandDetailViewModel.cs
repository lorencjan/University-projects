using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using DotVVM.Framework.ViewModel;
using DotVVM.Framework.Storage;
using DotVVM.Framework.Controls;
using Microsoft.Extensions.Logging;
using RockFests.BL.Model;
using RockFests.BL.Repositories;
using RockFests.BL.Resources;
using RockFests.DAL.Enums;
using System.Security.Cryptography.X509Certificates;

namespace RockFests.ViewModels.Bands
{
    public class BandDetailViewModel : MasterPageViewModel
    {
        private readonly BandRepository _bandRepository;
        private readonly BandRatingRepository _bandRatingRepository;
        private readonly MusicianRepository _musicianRepository;
        private readonly ILogger<BandDetailViewModel> _logger;
        private readonly IUploadedFileStorage _storage;
        public List<InterpretDto> Musicians { get; set; }

        public BandDto Band { get; set; } = new BandDto();
        public BandDto EditBand { get; set; }
        [FromRoute("Id")]
        public int BandId { get; set; }
        public string Base64Photo { get; set; }
        public string ModalDeleteMessage => string.Format(Texts.ModalDeleteBodyFormat, Band?.Name);

        public RatingDto MyRating { get; set; }
        public RatingDto EditMyRating { get; set; }
        public short[] RatingValues { get; set; } = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        public UploadedFilesCollection Files { get; set; } = new UploadedFilesCollection();
        public int SelectedMusicianId { get; set; }
        public BandDetailViewModel(BandRepository bandRepository, BandRatingRepository bandRatingRepository, MusicianRepository musicianRepository, ILogger<BandDetailViewModel> logger, IUploadedFileStorage storage)
        {
            _bandRepository = bandRepository;
            _bandRatingRepository = bandRatingRepository;
            _musicianRepository = musicianRepository;
            _logger = logger;
            _storage = storage;
        }

        public void NewPhotoUploaded()
        {
            Files.Files = new List<UploadedFile> { Files.Files[0] };
            var file = _storage.GetFile(Files.Files[0].FileId);
            EditBand.Photo = new byte[file.Length];
            file.Read(EditBand.Photo, 0, (int)file.Length);
            _storage.DeleteFile(Files.Files[0].FileId);
        }
        public override async Task Load()
        {
            await LoadBand();
            if (!Context.IsPostBack)
            {
                Musicians = (await _musicianRepository.GetAllLight()).Select(x => new InterpretDto { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name).ToList();
                if (Band.Members != null)
                {
                    foreach (MusicianLightDto member in Band.Members)
                    {
                        var foundMusician = Musicians.Find(x => x.Id == member.Id);
                        Musicians.Remove(foundMusician);
                    }
                }
            }
            Band.Ratings ??= new List<RatingDto>();
            MyRating = Band.Ratings.SingleOrDefault(x => x.UserName == SignedInUser?.Login);
            await base.Load();
        }

        private async Task LoadBand()
        {
            if (BandId == 0)
            {
                if (SignedInUser?.AccessRole != AccessRole.Admin && SignedInUser?.AccessRole != AccessRole.Organizer)
                    Context.RedirectToRoute(Routes.Bands);
                
                EditBand ??= new BandDto();
                return;
            }
            try
            {
                Band = await _bandRepository.GetById(BandId);
                if (Band == null)
                {
                    Context.RedirectToRoute(Routes.Bands);
                }
                Base64Photo = "data:image/png;base64," + Convert.ToBase64String(Band.Photo);
            }
            catch (Exception e)
            {
                SetError(e, Errors.BandLoad);
                _logger.LogError(e, Errors.BandLoad);
            }
        }

        public async Task SaveBand()
        {
            try
            {
                if (EditBand.Id > 0)
                {

                    await _bandRepository.Update(EditBand);
                    var newMusicians = EditBand.Members.Where(x => !Band.Members.Select(y => y.Id).Contains(x.Id)).Select(x => x.Id);
                    var deletedMusicians = Band.Members.Where(x => !EditBand.Members.Select(y => y.Id).Contains(x.Id)).Select(x => x.Id);
                    await _bandRepository.AddMusicians(BandId, newMusicians);
                    await _bandRepository.DeleteMusicians(BandId, deletedMusicians);

                    Context.RedirectToLocalUrl(Routes.Bands_Detail_Url + Band.Id);
                }
                else
                {
                    EditBand.Members ??= new List<MusicianLightDto>();
                    int newBandId = await _bandRepository.Add(EditBand);
                    await _bandRepository.AddMusicians(newBandId, EditBand.Members.Select(x => x.Id));

                    Context.RedirectToLocalUrl(Routes.Bands_Detail_Url + newBandId);
                }

                Band = EditBand;
                EditBand = null;
            }
            catch (Exception e)
                {
                SetError(e, Errors.BandSave);
                _logger.LogError(e, Errors.BandSave);
            }
        }

        public async Task DeleteBand()
        {
            try
            {
                await _bandRepository.Delete(BandId);
                Context.RedirectToRoute(Routes.Bands);
            }
            catch (Exception e)
            {
                SetError(e, Errors.BandDelete);
                _logger.LogError(e, Errors.BandDelete);
            }
        }

        public void CopyMyRating()
        {
            EditMyRating = new RatingDto
            {
                Id = MyRating.Id,
                UserName = MyRating.UserName,
                Number = MyRating.Number,
                Text = MyRating.Text,
                InterpretId = MyRating.InterpretId,
                InterpretName = MyRating.InterpretName
            };
        }

        public RatingDto NewRating() => new RatingDto
        {
            Id = 0,
            UserName = SignedInUser.Login,
            Number = 0,
            Text = string.Empty,
            InterpretId = Band.Id,
            InterpretName = Band.Name
        };

        public async Task CreateRating()
        {
            try
            {
                await _bandRatingRepository.Add(EditMyRating);
                MyRating = EditMyRating;
                await LoadBand();
            }
            catch (Exception e)
            {
                SetError(e, Errors.RatingCreate);
                _logger.LogError(e, Errors.RatingCreate);
            }
            finally
            {
                EditMyRating = null;
            }
        }

        public async Task UpdateMyRating()
        {
            try
            {
                await _bandRatingRepository.Update(EditMyRating);
                MyRating = EditMyRating;
            }
            catch (Exception e)
            {
                SetError(e, Errors.RatingUpdate);
                _logger.LogError(e, Errors.RatingUpdate);
            }
            finally
            {
                EditMyRating = null;
            }
        }

        public async Task DeleteRating(int id)
        {
            try
            {
                await _bandRatingRepository.Delete(id);

                if (id == MyRating.Id)
                    MyRating = null;
                else
                    Band.Ratings.RemoveAll(x => x.Id == id);

                await LoadBand();
            }
            catch (Exception e)
            {
                SetError(e, Errors.RatingDelete);
                _logger.LogError(e, Errors.RatingDelete);
            }
            finally
            {
                EditMyRating = null;
            }
        }

        public async Task DeleteMember(MusicianLightDto member)
        {
            try
            {
                var deleteMember = EditBand.Members.Find(x => x.Id == member.Id);
                EditBand.Members.Remove(deleteMember);
                Musicians.Add(new InterpretDto() { 
                    Id = member.Id,
                    Name = member.Name
                });;
            }
            catch(Exception e)
            {
                SetError(e, Errors.MemberRemove);
                _logger.LogError(e, Errors.MemberRemove);
            }
        }

        public async Task AddMember()
        {
            try 
            {
                EditBand.Members ??= new List<MusicianLightDto>();
                var selectedMusician = await _musicianRepository.GetById(SelectedMusicianId);
                EditBand.Members.Add(new MusicianLightDto() { 
                    Id = selectedMusician.Id,
                    Name = selectedMusician.FirstName + " " + selectedMusician.LastName
                });
                var selectedInterpret = Musicians.Find(x => x.Id == SelectedMusicianId);
                Musicians.Remove(selectedInterpret);
;            }
            catch (Exception e)
            {
                SetError(e, Errors.MemberRemove);
                _logger.LogError(e, Errors.MemberRemove);
            }
        }
    }
}
