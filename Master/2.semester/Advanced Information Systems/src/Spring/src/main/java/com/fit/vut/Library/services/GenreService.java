package com.fit.vut.Library.services;

import com.fit.vut.Library.dtos.GenreDto;
import com.fit.vut.Library.entities.Genre;
import com.fit.vut.Library.repositories.GenreRepository;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.*;

@Service
@Transactional
public class GenreService {

    private final GenreRepository repo;

    public GenreService(GenreRepository repo) {
        this.repo = repo;
    }

    public void save (GenreDto genre){
        repo.save(genre.toEntity());
    }

    public void delete (long id){ repo.delete(new Genre(id)); }

    public GenreDto getById (Long id){
        Optional<Genre> genre = repo.findById(id);
        return genre.isEmpty() ? null : genre.get().toDto();
    }

    public List<GenreDto> get (String name){
        List<Genre> genres = name == null ? repo.findAll() : repo.findByName(name);
        return genres.stream().map(x -> x.toDto()).toList();
    }
}
