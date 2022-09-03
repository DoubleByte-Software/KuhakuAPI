﻿using AutoMapper;
using K_haku.Core.Application.Dtos.Movie;
using K_haku.Core.Application.Interface.Repositories.Cuevana;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace K_haku.Core.Application.Features.MovieList.Queries.GetAll
{
    public class GetAllMovieListQuery : IRequest<List<MovieListResponse>>
    {
        public DateTime? ReleaseDate { get; set; }
        public string? MovieName { get; set; }
    }

    public class GetAllMovieListQueryHandler : IRequestHandler<GetAllMovieListQuery, List<MovieListResponse>>
    {
        private readonly IMapper _mapper;
        private readonly IMovieListRepository _movieListRepository;
        public GetAllMovieListQueryHandler(IMapper mapper, IMovieListRepository movieListRepository)
        {
            _mapper = mapper;
            _movieListRepository = movieListRepository;
        }
        public async Task<List<MovieListResponse>> Handle(GetAllMovieListQuery request, CancellationToken cancellationToken)
        {
            var parameters = _mapper.Map<GetAllMovieListParameters>(request);
            var movieList = await GetAllWithIncludes(parameters);
            return movieList;
        }

        public async Task<List<MovieListResponse>> GetAllWithIncludes(GetAllMovieListParameters parameters)
        {
            var movies = await _movieListRepository.GetAllAsync();

            if(parameters.MovieName != null)
            {
                movies = movies.Where(x => x.title.Contains(parameters.MovieName)).ToList();
            }
            if(parameters.ReleaseDate != null)
            {
                movies = movies.Where(x => x.release_date.Year == parameters.ReleaseDate.Value.Year).ToList();
            }

            return _mapper.Map<List<MovieListResponse>>(movies);
        }
    }

}