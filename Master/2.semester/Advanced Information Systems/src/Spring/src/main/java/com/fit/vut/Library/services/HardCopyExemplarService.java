package com.fit.vut.Library.services;

import com.fit.vut.Library.dtos.HardCopyExemplarDto;
import com.fit.vut.Library.entities.HardCopyExemplar;
import com.fit.vut.Library.repositories.HardCopyExemplarRepository;
import com.fit.vut.Library.services.filters.ExemplarFilter;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import javax.persistence.EntityManager;
import javax.persistence.PersistenceContext;
import javax.persistence.criteria.*;
import java.util.*;

@Service
@Transactional
public class HardCopyExemplarService {

    @PersistenceContext
    private EntityManager em;
    private final HardCopyExemplarRepository repo;

    public HardCopyExemplarService(HardCopyExemplarRepository repo) {
        this.repo = repo;
    }

    public void save (HardCopyExemplarDto exemplar){
        repo.save(exemplar.toEntity());
    }

    public void delete (Long id){
        repo.delete(new HardCopyExemplar(id));
    }

    public HardCopyExemplarDto getById (Long id){
        Optional<HardCopyExemplar> exemplar = repo.findById(id);
        return exemplar.isEmpty() ? null : exemplar.get().toDto();
    }

    public List<HardCopyExemplarDto> get(ExemplarFilter filter) {
        if (filter == null)
            return repo.findAll().stream().map(x -> x.toDto()).toList();

        CriteriaBuilder cb = em.getCriteriaBuilder();
        CriteriaQuery<HardCopyExemplar> cq = cb.createQuery(HardCopyExemplar.class);
        Root<HardCopyExemplar> quest = cq.from(HardCopyExemplar.class);
        List<Predicate> predicates = new ArrayList();

        if (filter.getState() != null)
            predicates.add(cb.like(quest.get("state"), "%" + filter.getState() + "%"));

        if (filter.getAvailability().isPresent())
            predicates.add(cb.equal(quest.get("availability"), filter.getAvailability().get()));

        cq.select(quest).where(predicates.toArray(new Predicate[] {}));
        List<HardCopyExemplar> exemplars = em.createQuery(cq).getResultList();

        if (filter.getTitle() != null)
            exemplars = exemplars.stream()
                    .filter(e -> (e.getBook() == null ? e.getMagazine() : e.getBook()).getName().contains(filter.getTitle()))
                    .toList();

        return exemplars.stream().map(x -> x.toDto()).toList();
    }
}
