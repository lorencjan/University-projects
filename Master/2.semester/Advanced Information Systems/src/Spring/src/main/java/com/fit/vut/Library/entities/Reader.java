package com.fit.vut.Library.entities;

import com.fit.vut.Library.dtos.*;
import com.fit.vut.Library.enums.RoleEnum;

import javax.persistence.*;
import java.util.*;

@Entity
@Table(name = "reader")
public class Reader extends User{

    @OneToMany(mappedBy = "reader", cascade = CascadeType.ALL, orphanRemoval = true)
    private List<Reservation> reservations = new ArrayList<>();

    @OneToMany(mappedBy = "reader", cascade = CascadeType.ALL, orphanRemoval = true)
    private List<HardCopyBorrowing> hardCopyBorrowings = new ArrayList<>();

    @OneToMany(mappedBy = "reader", cascade = CascadeType.ALL, orphanRemoval = true)
    private List<ElectronicCopyBorrowing> electronicCopyBorrowings = new ArrayList<>();

    public Reader() {}

    public Reader(Long id) {
        this.id = id;
    }

    public Reader(Long id, String email, String hashPassword, String name, String surname, String street, String houseNumber,
                  String city, String postcode, RoleEnum role,
                  List<Reservation> reservations, List<HardCopyBorrowing> hardCopyBorrowings,
                  List<ElectronicCopyBorrowing> electronicCopyBorrowings) {
        super(id, email, hashPassword, name, surname, street, houseNumber, city, postcode, role);
        this.reservations = reservations;
        this.hardCopyBorrowings = hardCopyBorrowings;
        this.electronicCopyBorrowings = electronicCopyBorrowings;
    }

    public Reader(String email, String hashPassword, String name, String surname, String street, String houseNumber, String city, String postcode, RoleEnum role,
                  List<Reservation> reservations, List<HardCopyBorrowing> hardCopyBorrowings, List<ElectronicCopyBorrowing> electronicCopyBorrowings) {
        super(email, hashPassword, name, surname, street, houseNumber, city, postcode, role);
        this.reservations = reservations;
        this.hardCopyBorrowings = hardCopyBorrowings;
        this.electronicCopyBorrowings = electronicCopyBorrowings;
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

    public List<ElectronicCopyBorrowing> getElectronicCopyBorrowings() {
        return electronicCopyBorrowings;
    }

    public void setElectronicCopyBorrowings(List<ElectronicCopyBorrowing> electronicCopyBorrowings) {
        this.electronicCopyBorrowings = electronicCopyBorrowings;
    }

    public void addReservations(Reservation reservation) {
        reservations.add(reservation);
        reservation.setReader(this);
    }

    public void removeReservations(Reservation reservation) {
        reservations.remove(reservation);
        reservation.setReader(this);
    }

    public void addHardCopyBorrowings(HardCopyBorrowing hardCopyBorrowing) {
        hardCopyBorrowings.add(hardCopyBorrowing);
        hardCopyBorrowing.setReader(this);
    }

    public void removeHardCopyBorrowings(HardCopyBorrowing hardCopyBorrowing) {
        hardCopyBorrowings.remove(hardCopyBorrowing);
        hardCopyBorrowing.setReader(this);
    }

    public void addElectronicCopyBorrowings(ElectronicCopyBorrowing electronicCopyBorrowing) {
        electronicCopyBorrowings.add(electronicCopyBorrowing);
        electronicCopyBorrowing.setReader(this);
    }

    public void removeElectronicCopyBorrowings(ElectronicCopyBorrowing electronicCopyBorrowing) {
        electronicCopyBorrowings.remove(electronicCopyBorrowing);
        electronicCopyBorrowing.setReader(this);
    }

    public ReaderLightDto toLightDto() { return new ReaderLightDto(id, name, surname); }

    public ReaderDto toDto() {
        List<ReservationDto> reservationDtos = getReservations().stream().map(x -> x.toDto()).toList();
        List<HardCopyBorrowingDto> borrowingDtos = getHardCopyBorrowings().stream().map(x -> x.toDto()).toList();
        List<ElectronicCopyBorrowingDto> eBorrowingDtos = getElectronicCopyBorrowings().stream().map(x -> x.toDto()).toList();
        return new ReaderDto(id, name, surname, email, city, street, houseNumber, postcode, role, reservationDtos, borrowingDtos, eBorrowingDtos); }
}
