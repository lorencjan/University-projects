package com.fit.vut.Library.dtos;

import com.fit.vut.Library.entities.*;
import com.fit.vut.Library.enums.ExemplarEnum;

import java.util.*;

public class HardCopyExemplarDto extends ExemplarBaseDto {

    private List<ReservationDto> reservations;

    private List<HardCopyBorrowingDto> borrowings;

    public HardCopyExemplarDto() {}

    public HardCopyExemplarDto(Long id, ExemplarEnum state, boolean availability, int borrowPeriod, int maximumNumberOfExtension, TitleLightDto book, TitleLightDto magazine,
                               List<ReservationDto> reservations, List<HardCopyBorrowingDto> borrowings) {
        super(id, state, availability, borrowPeriod, maximumNumberOfExtension, book, magazine);
        this.reservations = reservations;
        this.borrowings = borrowings;
    }

    public List<ReservationDto> getReservations() {
        return reservations;
    }

    public void setReservations(List<ReservationDto> reservations) {
        this.reservations = reservations;
    }

    public List<HardCopyBorrowingDto> getBorrowings() {
        return borrowings;
    }

    public void setBorrowings(List<HardCopyBorrowingDto> borrowings) {
        this.borrowings = borrowings;
    }

    public HardCopyExemplar toEntity() {
        Book bookEntity = getBook() == null ? null : getBook().toBookEntity();
        Magazine magazineEntity = getMagazine() == null ? null : getMagazine().toMagazineEntity();
        List<Reservation> reservationEntities = getReservations() == null ? new ArrayList() : getReservations().stream().map(x -> x.toEntity()).toList();
        List<HardCopyBorrowing> borrowingEntities = getBorrowings() == null ? new ArrayList() : getBorrowings().stream().map(x -> x.toEntity()).toList();
        return new HardCopyExemplar(id, state, availability, borrowPeriod, maximumNumberOfExtension,
                bookEntity, magazineEntity, reservationEntities, borrowingEntities);
    }
}
