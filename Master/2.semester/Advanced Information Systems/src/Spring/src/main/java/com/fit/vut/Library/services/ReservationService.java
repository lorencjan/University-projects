package com.fit.vut.Library.services;

import com.fit.vut.Library.dtos.ReservationDto;
import com.fit.vut.Library.entities.Reservation;
import com.fit.vut.Library.repositories.ReservationRepository;
import com.fit.vut.Library.services.filters.ReservationFilter;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import javax.persistence.EntityManager;
import javax.persistence.PersistenceContext;
import javax.persistence.criteria.*;
import java.util.*;

@Service
@Transactional
public class ReservationService {

    @PersistenceContext
    private EntityManager em;
    private final ReservationRepository repo;

    public ReservationService(ReservationRepository repo) {
        this.repo = repo;
    }

    public void save (ReservationDto reservation){
        repo.save(reservation.toEntity());
    }

    public void delete (Long id){
        repo.delete(new Reservation(id));
    }

    public ReservationDto getById (Long id){
        Optional<Reservation> reservation = repo.findById(id);
        return reservation.isEmpty() ? null : reservation.get().toDto();
    }

    public List<ReservationDto> get(ReservationFilter filter) {
        if (filter == null)
            return repo.findAll().stream().map(x -> x.toDto()).toList();

        CriteriaBuilder cb = em.getCriteriaBuilder();
        CriteriaQuery<Reservation> cq = cb.createQuery(Reservation.class);
        Root<Reservation> quest = cq.from(Reservation.class);
        List<Predicate> predicates = new ArrayList<Predicate>();

        if (filter.getStartedAfter() != null)
            predicates.add(cb.greaterThanOrEqualTo(quest.get("dateFrom").as(Date.class), filter.getStartedAfter()));

        if (filter.getEndedAfter() != null)
            predicates.add(cb.greaterThanOrEqualTo(quest.get("dateUntil").as(Date.class), filter.getEndedAfter()));

        if (filter.getState() != null)
            predicates.add(cb.like(quest.get("state"), "%" + filter.getState() + "%"));

        cq.select(quest).where(predicates.toArray(new Predicate[] {}));
        List<Reservation> reservations = em.createQuery(cq).getResultList();
        return reservations.stream().map(x -> x.toDto()).toList();
    }
}