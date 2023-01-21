package com.fit.vut.Library.services;

import com.fit.vut.Library.dtos.FineDto;
import com.fit.vut.Library.entities.Fine;
import com.fit.vut.Library.repositories.FineRepository;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.*;

@Service
@Transactional
public class FineService {

    private final FineRepository repo;

    public FineService(FineRepository repo) {
        this.repo = repo;
    }

    public void save (FineDto fine){
        repo.save(fine.toEntity());
    }

    public void delete (Long id){
        repo.delete(new Fine(id));
    }

    public FineDto getById (Long id){
        Optional<Fine> fine = repo.findById(id);
        return fine.isEmpty() ? null : fine.get().toDto();
    }

    public List<FineDto> get(Optional<Integer> state) {
        List<Fine> fines = state.isPresent() ? repo.findByState(state.get()) : repo.findAll();
        return fines.stream().map(x -> x.toDto()).toList();
    }
}
