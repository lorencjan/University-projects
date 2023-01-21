package com.fit.vut.Library.services;

import com.fit.vut.Library.dtos.HardCopyBorrowingDto;
import com.fit.vut.Library.entities.*;
import com.fit.vut.Library.enums.HardCopyBorrowingEnum;
import com.fit.vut.Library.repositories.HardCopyBorrowingRepository;
import com.fit.vut.Library.repositories.HardCopyExemplarRepository;
import com.fit.vut.Library.repositories.ReservationRepository;
import com.fit.vut.Library.services.filters.BorrowingFilter;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import javax.persistence.EntityManager;
import javax.persistence.PersistenceContext;
import javax.persistence.criteria.*;
import java.time.*;
import java.time.temporal.ChronoUnit;
import java.util.*;

@Service
@Transactional
public class HardCopyBorrowingService {

    @PersistenceContext
    private EntityManager em;
    private final HardCopyBorrowingRepository repo;
    private final ReservationRepository reservationRepository;
    private final HardCopyExemplarRepository hardCopyExemplarRepository;

      public HardCopyBorrowingService(HardCopyBorrowingRepository repo,
                                    ReservationRepository reservationRepository,
                                      HardCopyExemplarRepository hardCopyExemplarRepository) {
        this.repo = repo;
        this.reservationRepository = reservationRepository;
        this.hardCopyExemplarRepository = hardCopyExemplarRepository;
    }

    public void save (HardCopyBorrowingDto borrowing){
        repo.save(borrowing.toEntity());
    }

    public void delete (Long id){
        repo.delete(new HardCopyBorrowing(id));
    }

    public HardCopyBorrowingDto getById (Long id){
        Optional<HardCopyBorrowing> borrowing = repo.findById(id);
        return borrowing.isEmpty() ? null : borrowing.get().toDto();
    }

    public List<HardCopyBorrowingDto> get(BorrowingFilter filter) {
        if (filter == null)
            return repo.findAll().stream().map(x -> x.toDto()).toList();

        CriteriaBuilder cb = em.getCriteriaBuilder();
        CriteriaQuery<HardCopyBorrowing> cq = cb.createQuery(HardCopyBorrowing.class);
        Root<HardCopyBorrowing> quest = cq.from(HardCopyBorrowing.class);
        List<Predicate> predicates = new ArrayList();

        if (filter.getStartedAfter() != null)
            predicates.add(cb.greaterThanOrEqualTo(quest.get("dateOfBorrowStart").as(Date.class), filter.getStartedAfter()));

        if (filter.getEndedAfter() != null)
            predicates.add(cb.greaterThanOrEqualTo(quest.get("dateOfBorrowEnd").as(Date.class), filter.getEndedAfter()));

        if (filter.getState() != null)
            predicates.add(cb.like(quest.get("state"), "%" + filter.getState() + "%"));

        cq.select(quest).where(predicates.toArray(new Predicate[] {}));
        List<HardCopyBorrowing> hardCopyBorrowings = em.createQuery(cq).getResultList();
        return hardCopyBorrowings.stream().map(x -> x.toDto()).toList();
    }

    public List<HardCopyBorrowingDto> getBorrowingsThatExpireIn(int days){
        CriteriaBuilder cb = em.getCriteriaBuilder();
        CriteriaQuery<HardCopyBorrowing> cq = cb.createQuery(HardCopyBorrowing.class);
        Root<HardCopyBorrowing> quest = cq.from(HardCopyBorrowing.class);
        List<Predicate> predicates = new ArrayList();

        // we want to get those which ends in today + days (whole day period
        predicates.add(cb.greaterThanOrEqualTo(quest.get("dateOfBorrowEnd").as(LocalDate.class), LocalDate.now().plusDays(days)));
        predicates.add(cb.lessThan(quest.get("dateOfBorrowEnd").as(LocalDate.class), LocalDate.now().plusDays(days + 1)));
        predicates.add(cb.isNull(quest.get("returnDate")));

        cq.select(quest).where(predicates.toArray(new Predicate[] {}));
        List<HardCopyBorrowing> hardCopyBorrowings = em.createQuery(cq).getResultList();
        return hardCopyBorrowings.stream().map(x -> x.toDto()).toList();
    }

    public List<HardCopyBorrowingDto> getBorrowingsWithExpiredFine(int finePeriod){
        CriteriaBuilder cb = em.getCriteriaBuilder();
        CriteriaQuery<HardCopyBorrowing> cq = cb.createQuery(HardCopyBorrowing.class);
        Root<HardCopyBorrowing> quest = cq.from(HardCopyBorrowing.class);
        List<Predicate> predicates = new ArrayList();

        // we want to get all expired and not yet returned borrowings
        predicates.add(cb.lessThan(quest.get("dateOfBorrowEnd").as(LocalDate.class), LocalDate.now()));
        predicates.add(cb.isNull(quest.get("returnDate")));

        cq.select(quest).where(predicates.toArray(new Predicate[] {}));
        List<HardCopyBorrowing> hardCopyBorrowings = em.createQuery(cq).getResultList();

        // filter additionally those to which a new fine should be charged
        return hardCopyBorrowings.stream()
                .filter(x -> expiredFinePredicate(x, finePeriod))
                .map(x -> x.toDto())
                .toList();
    }

    private Boolean expiredFinePredicate(HardCopyBorrowing borrowing, int finePeriod) {
        LocalDate now = LocalDate.now();
        LocalDate borrowingEnd = borrowing.getDateOfBorrowEnd().toInstant().atZone(ZoneId.systemDefault()).toLocalDate();
        long days = ChronoUnit.DAYS.between(borrowingEnd, now);
        return days % finePeriod == 0;
    }

    public Boolean prolong(HardCopyBorrowingDto borrowing) {
        Boolean canProlong = this.checkProlongPossible(borrowing.getId(), borrowing.getDateOfBorrowEnd());
        if (canProlong) {
            HardCopyBorrowing hardCopyBorrowing = borrowing.toEntity();
            HardCopyExemplar hardCopyExemplar = hardCopyExemplarRepository.findById(hardCopyBorrowing.getHardCopyExemplar().getId()).get();
            Integer maxProlong = hardCopyExemplar.getMaximumNumberOfExtension();
            if (hardCopyBorrowing.getExtensionCounter() > hardCopyExemplar.getMaximumNumberOfExtension()){
                return false;
            }
            if (hardCopyBorrowing.getExtensionCounter() == hardCopyExemplar.getMaximumNumberOfExtension()){
                hardCopyBorrowing.setState(HardCopyBorrowingEnum.CAN_NOT_PROLONG);
            }
            repo.save(hardCopyBorrowing);
            return true;
        }
        return false;
      }

    private boolean checkProlongPossible(Long borrowing_id, Date dateTo){
        HardCopyBorrowing borrowing = repo.findById(borrowing_id).get();
        HardCopyExemplar exemplar = borrowing.getHardCopyExemplar();
        List<Reservation> reservations = reservationRepository.findByHardCopyExemplar(exemplar);
        Boolean prolong = true;
        if (reservations.isEmpty()) prolong=true;
        else {
            for ( Reservation res : reservations) {
                if (! res.getDateFrom().toInstant().isAfter(dateTo.toInstant())){
                    prolong=false;
                    break;
                }
            }
        }
        return prolong;
    }
}