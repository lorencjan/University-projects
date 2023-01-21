using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotVVM.Framework.ViewModel;
using DotVVM.Framework.Storage;
using DotVVM.Framework.Controls;
using Microsoft.Extensions.Logging;
using RockFests.BL.Model;
using RockFests.BL.Repositories;
using RockFests.BL.Resources;
using RockFests.DAL.Enums;
using System.Linq;

namespace RockFests.ViewModels.Musicians
{
    public class MusicianDetailViewModel : MasterPageViewModel
    {
        private readonly MusicianRepository _musicianRepository;
        private readonly MusicianRatingRepository _musicianRatingRepository;
        private readonly ILogger<MusicianDetailViewModel> _logger;

        public MusicianDto Musician { get; set; } = new MusicianDto();
        public MusicianDto EditMusician { get; set; }
        [FromRoute("Id")]
        public int MusicianId { get; set; }

        public string Base64Photo { get; set; } = string.Empty;
        public string ModalDeleteMessage => string.Format(Texts.ModalDeleteBodyFormat, $"{Musician?.FirstName} {Musician?.LastName}");

        public UploadedFilesCollection Files { get; set; } = new UploadedFilesCollection();
        private readonly IUploadedFileStorage _storage;

        public RatingDto MyRating { get; set; }
        public RatingDto EditMyRating { get; set; }
        public short[] RatingValues { get; set; } = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        public MusicianDetailViewModel(MusicianRepository musicianRepository, MusicianRatingRepository musicianRatingRepository, ILogger<MusicianDetailViewModel> logger, IUploadedFileStorage storage)
        {
            _musicianRepository = musicianRepository;
            _musicianRatingRepository = musicianRatingRepository;
            _logger = logger;
            _storage = storage;
        }

        public override async Task Load()
        {
            await LoadMusician();
            Musician.Ratings ??= new List<RatingDto>();
            MyRating = Musician.Ratings.SingleOrDefault(x => x.UserName == SignedInUser?.Login);
            await base.Load();
        }

        private async Task LoadMusician()
        {
            if (MusicianId == 0)
            {
                if (SignedInUser?.AccessRole != AccessRole.Admin && SignedInUser?.AccessRole != AccessRole.Organizer)
                    Context.RedirectToRoute(Routes.Musicians);

                EditMusician ??= new MusicianDto();
                return;
            }

            try
            {
                Musician = await _musicianRepository.GetById(MusicianId);
                if (Musician == null)
                {
                    Context.RedirectToRoute(Routes.Musicians);
                }
                Base64Photo = "data:image/png;base64," + Convert.ToBase64String(Musician.Photo);
            }
            catch (Exception e)
            {
                SetError(e, Errors.MusicianLoad);
                _logger.LogError(e, Errors.MusicianLoad);
            }
        }

        public async Task CreateMusician()
        {
            try
            {
                var newMusicianId = await _musicianRepository.Add(EditMusician);
                Context.RedirectToLocalUrl(Routes.Musicians_Detail_Url + newMusicianId);
            }
            catch (Exception e)
            {
                SetError(e, Errors.MusicianCreate);
                _logger.LogError(e, Errors.MusicianCreate);
            }
        }

        public async Task UpdateMusician()
        {
            try
            {
                await _musicianRepository.Update(EditMusician);
                Context.RedirectToLocalUrl(Routes.Musicians_Detail_Url + MusicianId);
            }
            catch (Exception e)
            {
                SetError(e, Errors.MusicianUpdate);
                _logger.LogError(e, Errors.MusicianUpdate);
            }
        }

        public async Task DeleteMusician()
        {
            try
            {
                await _musicianRepository.Delete(MusicianId);
                Context.RedirectToRoute(Routes.Musicians);
            }
            catch (Exception e)
            {
                SetError(e, Errors.MusicianDelete);
                _logger.LogError(e, Errors.MusicianDelete);
            }
        }

        public void NewPhotoUploaded()
        {
            Files.Files = new List<UploadedFile> { Files.Files[0] };
            var file = _storage.GetFile(Files.Files[0].FileId);
            EditMusician.Photo = new byte[file.Length];
            file.Read(EditMusician.Photo, 0, (int)file.Length);
            _storage.DeleteFile(Files.Files[0].FileId);
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
            InterpretId = Musician.Id
        };

        public async Task CreateRating()
        {
            try
            {
                await _musicianRatingRepository.Add(EditMyRating);
                MyRating = EditMyRating;
                await LoadMusician();
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
                await _musicianRatingRepository.Update(EditMyRating);
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
                await _musicianRatingRepository.Delete(id);

                if (id == MyRating.Id)
                    MyRating = null;
                else
                    Musician.Ratings.RemoveAll(x => x.Id == id);

                await LoadMusician();
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
    }
}
