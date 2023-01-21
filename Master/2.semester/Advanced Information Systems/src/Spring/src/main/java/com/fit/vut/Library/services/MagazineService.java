package com.fit.vut.Library.services;

import com.fit.vut.Library.dtos.MagazineDto;
import com.fit.vut.Library.entities.Magazine;
import com.fit.vut.Library.repositories.MagazineRepository;
import com.fit.vut.Library.services.filters.MagazineFilter;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import javax.persistence.EntityManager;
import javax.persistence.PersistenceContext;
import javax.persistence.criteria.*;
import java.util.*;

@Service
@Transactional
public class MagazineService {

    @PersistenceContext
    private EntityManager em;
    private final MagazineRepository repo;

    public MagazineService(MagazineRepository repo) {
        this.repo = repo;
    }

    public void save (MagazineDto magazine){
        repo.save(magazine.toEntity());
    }

    public void delete (Long id){
        repo.delete(new Magazine(id));
    }

    public MagazineDto getById (Long id){
        Optional<Magazine> magazine = repo.findById(id);
        return magazine.isEmpty() ? null : magazine.get().toDto();
    }

    public List<MagazineDto> get(MagazineFilter filter) {
        if (filter == null)
            return repo.findAll().stream().map(x -> x.toDto()).toList();

        CriteriaBuilder cb = em.getCriteriaBuilder();
        CriteriaQuery<Magazine> cq = cb.createQuery(Magazine.class);
        Root<Magazine> quest = cq.from(Magazine.class);
        List<Predicate> predicates = new ArrayList();

        if (filter.getName() != null)
            predicates.add(cb.like(quest.get("name"), "%" + filter.getName() + "%"));

        if (filter.getPublisher() != null)
            predicates.add(cb.like(quest.get("publisher"), "%" + filter.getPublisher() + "%"));

        if (filter.getLanguage() != null)
            predicates.add(cb.like(quest.get("language"), "%" + filter.getLanguage() + "%"));

        if (filter.getIssn() != null)
            predicates.add(cb.equal(quest.get("issn"), filter.getIssn()));

        if (filter.getYear().isPresent())
            predicates.add(cb.equal(quest.get("year"), filter.getYear().get()));

        if (filter.getNumber().isPresent())
            predicates.add(cb.equal(quest.get("number"), filter.getNumber().get()));

        cq.select(quest).where(predicates.toArray(new Predicate[] {}));
        List<Magazine> magazines = em.createQuery(cq).getResultList();

        if (filter.getAuthor() != null) {
            magazines = magazines.stream()
                    .filter(b -> b.getAuthors().stream()
                            .anyMatch(a -> a.getName().contains(filter.getAuthor())
                                    || a.getSurname().contains(filter.getAuthor()))).toList();
        }

        if (filter.getFields() != null && !filter.getFields().isEmpty())
            magazines = magazines.stream().filter(b -> b.getFields().stream().anyMatch(g -> filter.getFields().contains(g.getName()))).toList();

        return magazines.stream().map(x -> x.toDto()).toList();
    }
}
