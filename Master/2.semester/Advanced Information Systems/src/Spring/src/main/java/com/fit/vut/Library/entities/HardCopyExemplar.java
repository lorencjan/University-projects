package com.fit.vut.Library.entities;

import com.fit.vut.Library.dtos.*;
import com.fit.vut.Library.enums.ExemplarEnum;

import java.util.*;
import javax.persistence.*;

@Entity
@Table(name = "hardcopyexemplar")
public class HardCopyExemplar extends Exemplar {

    @OneToMany(mappedBy = "hardCopyExemplar", cascade = CascadeType.ALL, orphanRemoval = true)
    private List<Reservation> reservations = new ArrayList<>();

    @OneToMany(mappedBy = "hardCopyExemplar",  cascade = CascadeType.ALL, orphanRemoval = true)
    private List<HardCopyBorrowing> hardCopyBorrowings = new ArrayList<>();

    public HardCopyExemplar() {}

    public HardCopyExemplar(Long id) {
        this.id = id;
    }

    public HardCopyExemplar(Long id, ExemplarEnum state, boolean availability, int borrowPeriod, int maximumNumberOfBorrow,
                            Book book, Magazine magazine, List<Reservation> reservations, List<HardCopyBorrowing> borrowings) {
        super(id, state, availability, borrowPeriod, maximumNumberOfBorrow, book, magazine);
        this.reservations = reservations;
        this.hardCopyBorrowings = borrowings;
    }

    public HardCopyExemplar(ExemplarEnum state, boolean availability, int borrowPeriod, int maximumNumberOfBorrow,
                            Book book, Magazine magazine, List<Reservation> reservations, List<HardCopyBorrowing> borrowings) {
        super(state, availability, borrowPeriod, maximumNumberOfBorrow, book, magazine);
        this.reservations = reservations;
        this.hardCopyBorrowings = borrowings;
    }

    public List<Reservation> getReservations() {
        return reservations;
    }

    public void setReservations(List<Reservation> reservations) {
        this.reservations = reservations;
    }

    public List<HardCopyBorrowing> getHardCopyBorrowings() {
        return hardCopyBorrowings;
    }

    public void setHardCopyBorrowings(List<HardCopyBorrowing> hardCopyBorrowings) {
        this.hardCopyBorrowings = hardCopyBorrowings;
    }

    public void addReservations(Reservation reservation) {
        reservations.add(reservation);
        reservation.setHardCopyExemplar(this);
    }

    public void removeReservations(Reservation reservation) {
        reservations.remove(reservation);
        reservation.setHardCopyExemplar(this);
    }

    public void addHardCopyBorrowings(HardCopyBorrowing hardCopyBorrowing) {
        hardCopyBorrowings.add(hardCopyBorrowing);
        hardCopyBorrowing.setHardCopyExemplar(this);
    }

    public void removeHardCopyBorrowings(HardCopyBorrowing hardCopyBorrowing) {
        hardCopyBorrowings.remove(hardCopyBorrowing);
        hardCopyBorrowing.setHardCopyExemplar(this);
    }

    public HardCopyExemplarDto toDto() {
        List<ReservationDto> reservationEntities = getReservations() == null ? new ArrayList() : getReservations().stream().map(x -> x.toDto()).toList();
        List<HardCopyBorrowingDto> borrowingEntities = getHardCopyBorrowings() == null ? new ArrayList() : getHardCopyBorrowings().stream().map(x -> x.toDto()).toList();
        TitleLightDto bookDto = book == null ? null : book.toLightDto();
        TitleLightDto magazineDto = magazine == null ? null : magazine.toLightDto();
        return new HardCopyExemplarDto(id, state, availability, borrowPeriod, maximumNumberOfExtension, bookDto, magazineDto, reservationEntities, borrowingEntities);
    }
}
