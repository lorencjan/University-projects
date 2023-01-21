package com.fit.vut.Library.services;

import com.fit.vut.Library.dtos.FieldDto;
import com.fit.vut.Library.entities.Field;
import com.fit.vut.Library.repositories.FieldRepository;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.*;

@Service
@Transactional
public class FieldService {

    private final FieldRepository repo;

    public FieldService(FieldRepository repo) {
        this.repo = repo;
    }

    public void save (FieldDto field){
        repo.save(field.toEntity());
    }

    public void delete (long id){ repo.delete(new Field(id)); }

    public FieldDto getById (Long id){
        Optional<Field> field = repo.findById(id);
        return field.isEmpty() ? null : field.get().toDto();
    }

    public List<FieldDto> get (String name){
        List<Field> fields = name == null ? repo.findAll() : repo.findByName(name);
        return fields.stream().map(x -> x.toDto()).toList();
    }
}
