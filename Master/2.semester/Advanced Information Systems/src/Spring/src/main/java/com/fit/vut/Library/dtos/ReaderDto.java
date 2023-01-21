package com.fit.vut.Library.dtos;

import com.fit.vut.Library.entities.*;
import com.fit.vut.Library.enums.RoleEnum;

import java.util.*;

public class ReaderDto extends UserDto {

    public List<ReservationDto> reservations;

    public List<ElectronicCopyBorrowingDto> electronicCopyBorrowings;

    public List<HardCopyBorrowingDto> hardCopyBorrowings;

    public ReaderDto() {}

    public ReaderDto(Long id, String name, String surname, String email, String city, String street, String houseNumber, String postcode, RoleEnum role,
                     List<ReservationDto> reservations, List<HardCopyBorrowingDto> hardCopyBorrowings, List<ElectronicCopyBorrowingDto> electronicCopyBorrowings) {
        super(id, name, surname, email, city, street, houseNumber, postcode, role);
        this.reservations = reservations;
        this.hardCopyBorrowings = hardCopyBorrowings;
        this.electronicCopyBorrowings = electronicCopyBorrowings;
    }

    public List<ReservationDto> getReservations() { return reservations; }

    public void setReservations(List<ReservationDto> reservations) { this.reservations = reservations; }

    public List<ElectronicCopyBorrowingDto> getElectronicCopyBorrowings() { return electronicCopyBorrowings; }

    public void setElectronicCopyBorrowings(List<ElectronicCopyBorrowingDto> borrowings) { this.electronicCopyBorrowings = borrowings; }

    public List<HardCopyBorrowingDto> getHardCopyBorrowings() { return hardCopyBorrowings; }

    public void setHardCopyBorrowings(List<HardCopyBorrowingDto> borrowings) { this.hardCopyBorrowings = borrowings; }

    public Reader toEntity() {
        List<Reservation> reservations = getReservations() == null ? new ArrayList() : getReservations().stream().map(x -> x.toEntity()).toList();
        List<HardCopyBorrowing> borrowings = getHardCopyBorrowings() == null ? new ArrayList() : getHardCopyBorrowings().stream().map(x -> x.toEntity()).toList();
        List<ElectronicCopyBorrowing> eBorrowings = getElectronicCopyBorrowings() == null ? new ArrayList() : getElectronicCopyBorrowings().stream().map(x -> x.toEntity()).toList();
        return new Reader(id, email, "", name, surname, street, houseNumber, city, postcode, role, reservations, borrowings, eBorrowings);
    }
}
