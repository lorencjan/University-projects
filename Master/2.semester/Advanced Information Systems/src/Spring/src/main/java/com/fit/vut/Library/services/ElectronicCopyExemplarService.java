package com.fit.vut.Library.services;

import com.fit.vut.Library.dtos.ElectronicCopyExemplarDto;
import com.fit.vut.Library.entities.ElectronicCopyExemplar;
import com.fit.vut.Library.repositories.ElectronicCopyExemplarRepository;
import com.fit.vut.Library.services.filters.ExemplarFilter;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import javax.persistence.EntityManager;
import javax.persistence.PersistenceContext;
import javax.persistence.criteria.*;
import java.util.*;

@Service
@Transactional
public class ElectronicCopyExemplarService {

    @PersistenceContext
    private EntityManager em;
    private final ElectronicCopyExemplarRepository repo;

    public ElectronicCopyExemplarService(ElectronicCopyExemplarRepository repo) {
        this.repo = repo;
    }

    public void save (ElectronicCopyExemplarDto exemplar){
        repo.save(exemplar.toEntity());
    }

    public void delete (Long id){
        repo.delete(new ElectronicCopyExemplar(id));
    }

    public ElectronicCopyExemplarDto getById (Long id){
        Optional<ElectronicCopyExemplar> exemplar = repo.findById(id);
        return exemplar.isEmpty() ? null : exemplar.get().toDto();
    }

    public List<ElectronicCopyExemplarDto> get(ExemplarFilter filter) {
        if (filter == null)
            return repo.findAll().stream().map(x -> x.toDto()).toList();

        CriteriaBuilder cb = em.getCriteriaBuilder();
        CriteriaQuery<ElectronicCopyExemplar> cq = cb.createQuery(ElectronicCopyExemplar.class);
        Root<ElectronicCopyExemplar> quest = cq.from(ElectronicCopyExemplar.class);
        List<Predicate> predicates = new ArrayList();

        if (filter.getState() != null)
            predicates.add(cb.like(quest.get("state"), "%" + filter.getState() + "%"));

        if (filter.getAvailability().isPresent())
            predicates.add(cb.equal(quest.get("availability"), filter.getAvailability().get()));

        cq.select(quest).where(predicates.toArray(new Predicate[] {}));
        List<ElectronicCopyExemplar> exemplars = em.createQuery(cq).getResultList();

        if (filter.getTitle() != null)
            exemplars = exemplars.stream()
                    .filter(e -> (e.getBook() == null ? e.getMagazine() : e.getBook()).getName().contains(filter.getTitle()))
                    .toList();

        return exemplars.stream().map(x -> x.toDto()).toList();
    }
}
