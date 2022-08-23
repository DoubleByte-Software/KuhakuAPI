﻿using AutoMapper;
using K_haku.Core.Application.Interface.Repositories.Cuevana;
using K_haku.Core.Application.WebsScrapers.GetAll.Cuevana;
using K_haku.Core.Domain.Entities.Cuevana;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using K_haku.Core.Application.Helpers;

namespace K_haku.Core.Application.Features.Cuevana.Commands.GetCuevanaMovies
{
    public class GetCuevanaMoviesCommand : IRequest<bool> 
    {
        public bool Start { get; set; }
    }
    public class GetCuevanaMoviesCommandHandler : IRequestHandler<GetCuevanaMoviesCommand, bool>
    {
        private readonly ICuevanaMoviesRepository _cuevanaMoviesRepository;
        private readonly IMapper _mapper;
        public readonly GetTMDBInfo _getTMDBInfo;

        public GetCuevanaMoviesCommandHandler(ICuevanaMoviesRepository cuevanaMoviesRepository, IMapper mapper, GetTMDBInfo getTMDBInfo)
        {
            _cuevanaMoviesRepository = cuevanaMoviesRepository;
            _mapper = mapper;
            _getTMDBInfo = getTMDBInfo;
        }
        public async Task<bool> Handle(GetCuevanaMoviesCommand request, CancellationToken cancellationToken)
        {
            CuevanaGetAllMovies _cuevanaGetAllMovies = new();
            var CuevanaMoviesList = await _cuevanaGetAllMovies.MovieList();
            List<CuevanaMovies> NewMovies = new();
            int i = 0;
            foreach(var Movie in CuevanaMoviesList)
            {
                var convert = _mapper.Map<CuevanaMovies>(Movie);
                if (await _cuevanaMoviesRepository.Exist(convert) == false)
                {
                    i++;
                    Console.WriteLine($"converting data #{i} from {convert.Title} : {convert.Link}");
                    convert.TMDB = await _getTMDBInfo.GetTMDBId(Movie.Title);
                    convert.CreatedBy = "Kuhaku Scrapping";
                    convert.Created = DateTime.Now;
                    NewMovies.Add(convert);
                }
            }
            await _cuevanaMoviesRepository.AddAllAsync(NewMovies);
            return true;
        }
    }
}
