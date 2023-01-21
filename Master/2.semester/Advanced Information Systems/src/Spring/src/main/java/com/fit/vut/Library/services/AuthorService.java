package com.fit.vut.Library.services;

import com.fit.vut.Library.dtos.AuthorDto;
import com.fit.vut.Library.entities.Author;
import com.fit.vut.Library.repositories.AuthorRepository;
import com.fit.vut.Library.services.filters.AuthorFilter;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import javax.persistence.EntityManager;
import javax.persistence.PersistenceContext;
import javax.persistence.criteria.*;
import java.util.*;

@Service
@Transactional
public class AuthorService {

    @PersistenceContext
    private EntityManager em;
    private final AuthorRepository repo;

    public AuthorService(AuthorRepository repo) {
        this.repo = repo;
    }

    public void save (AuthorDto author){
        repo.save(author.toEntity());
    }

    public void delete (Long id){
        repo.delete(new Author(id));
    }

    public AuthorDto getById (Long id){
        Optional<Author> author = repo.findById(id);
        return author.isEmpty() ? null : author.get().toDto();
    }

    public List<AuthorDto> get(AuthorFilter filter) {
        if (filter == null)
            return repo.findAll().stream().map(x -> x.toDto()).toList();

        CriteriaBuilder cb = em.getCriteriaBuilder();
        CriteriaQuery<Author> cq = cb.createQuery(Author.class);
        Root<Author> quest = cq.from(Author.class);
        List<Predicate> predicates = new ArrayList();

        if (filter.getName() != null)
            predicates.add(cb.like(quest.get("name"), "%" + filter.getName() + "%"));

        if (filter.getSurname() != null)
            predicates.add(cb.like(quest.get("surname"), "%" + filter.getSurname() + "%"));

        cq.select(quest).where(predicates.toArray(new Predicate[] {}));
        List<Author> authors = em.createQuery(cq).getResultList();
        return authors.stream().map(x -> x.toDto()).toList();
    }
}
