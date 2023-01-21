package com.fit.vut.Library.services;

import com.fit.vut.Library.dtos.ElectronicCopyBorrowingDto;
import com.fit.vut.Library.entities.*;
import com.fit.vut.Library.repositories.ElectronicCopyBorrowingRepository;
import com.fit.vut.Library.services.filters.BorrowingFilter;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import javax.persistence.EntityManager;
import javax.persistence.PersistenceContext;
import javax.persistence.criteria.*;
import java.util.*;

@Service
@Transactional
public class ElectronicCopyBorrowingService {

    @PersistenceContext
    private EntityManager em;
    private final ElectronicCopyBorrowingRepository repo;

    public ElectronicCopyBorrowingService(ElectronicCopyBorrowingRepository repo) {
        this.repo = repo;
    }

    public void save (ElectronicCopyBorrowingDto borrowing){
        repo.save(borrowing.toEntity());
    }

    public void delete (Long id){
        repo.delete(new ElectronicCopyBorrowing(id));
    }

    public ElectronicCopyBorrowingDto getById (Long id){
        Optional<ElectronicCopyBorrowing> borrowing = repo.findById(id);
        return borrowing.isEmpty() ? null : borrowing.get().toDto();
    }

    public List<ElectronicCopyBorrowingDto> get(BorrowingFilter filter) {
        if (filter == null)
            return repo.findAll().stream().map(x -> x.toDto()).toList();

        CriteriaBuilder cb = em.getCriteriaBuilder();
        CriteriaQuery<ElectronicCopyBorrowing> cq = cb.createQuery(ElectronicCopyBorrowing.class);
        Root<ElectronicCopyBorrowing> quest = cq.from(ElectronicCopyBorrowing.class);
        List<Predicate> predicates = new ArrayList();

        if (filter.getStartedAfter() != null)
            predicates.add(cb.greaterThanOrEqualTo(quest.get("dateOfBorrowStart").as(Date.class), filter.getStartedAfter()));

        if (filter.getEndedAfter() != null)
            predicates.add(cb.greaterThanOrEqualTo(quest.get("dateOfBorrowEnd").as(Date.class), filter.getEndedAfter()));

        if (filter.getState() != null)
            predicates.add(cb.like(quest.get("state"), "%" + filter.getState() + "%"));

        cq.select(quest).where(predicates.toArray(new Predicate[] {}));
        List<ElectronicCopyBorrowing> electronicCopyBorrowings = em.createQuery(cq).getResultList();
        return electronicCopyBorrowings.stream().map(x -> x.toDto()).toList();
    }
}