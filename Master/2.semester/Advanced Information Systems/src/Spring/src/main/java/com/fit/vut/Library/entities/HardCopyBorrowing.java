package com.fit.vut.Library.entities;

import com.fit.vut.Library.dtos.*;
import com.fit.vut.Library.enums.HardCopyBorrowingEnum;

import javax.persistence.*;
import java.util.*;

@Entity
@Table(name = "hardcopyborrowing")
public class HardCopyBorrowing extends Borrowing{

    @Column(name = "state", nullable = false)
    @Enumerated(EnumType.STRING)
    private HardCopyBorrowingEnum state;

    @Column(name = "returnDate")
    private Date returnDate;

    @OneToMany(mappedBy = "hardCopyBorrowing", fetch = FetchType.LAZY, orphanRemoval = true, cascade = { CascadeType.PERSIST, CascadeType.MERGE })
    private List<Fine> fines = new ArrayList<>();

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name="hardCopyExemplarId", nullable = false)
    private HardCopyExemplar hardCopyExemplar;

    public HardCopyBorrowing() {}

    public HardCopyBorrowing(Long id) {
        this.id = id;
    }

    public HardCopyBorrowing(Long id, Date dateOfBorrowStart, Date dateOfBorrowEnd, int extensionCounter, Reader reader,
                             HardCopyBorrowingEnum state, Date returnDate, List<Fine> fines,
                             HardCopyExemplar hardCopyExemplar) {
        super(id, dateOfBorrowStart, dateOfBorrowEnd, extensionCounter, reader);
        this.state = state;
        this.returnDate = returnDate;
        this.fines = fines;
        this.hardCopyExemplar = hardCopyExemplar;
    }


    public HardCopyBorrowing(Date dateOfBorrowStart, Date dateOfBorrowEnd, int extensionCounter, Reader reader,
                             HardCopyBorrowingEnum state, Date returnDate, List<Fine> fines,
                             HardCopyExemplar hardCopyExemplar) {
        super(dateOfBorrowStart, dateOfBorrowEnd, extensionCounter, reader);
        this.state = state;
        this.returnDate = returnDate;
        this.fines = fines;
        this.hardCopyExemplar = hardCopyExemplar;
    }


    public HardCopyBorrowingEnum getState() {
        return state;
    }

    public void setState(HardCopyBorrowingEnum state) {
        this.state = state;
    }

    public Date getReturnDate() {
        return returnDate;
    }

    public void setReturnDate(Date returnDate) {
        this.returnDate = returnDate;
    }

    public List<Fine> getFines() {
        return fines;
    }

    public void setFines(List<Fine> fines) {
        this.fines = fines;
    }

    public HardCopyExemplar getHardCopyExemplar() {
        return hardCopyExemplar;
    }

    public void setHardCopyExemplar(HardCopyExemplar hardCopyExemplar) {
        this.hardCopyExemplar = hardCopyExemplar;
    }

    public HardCopyBorrowingDto toDto() {
        List<FineDto> fineDtos = getFines() == null ? new ArrayList() : getFines().stream().map(x -> x.toDto()).toList();
        return new HardCopyBorrowingDto(id, dateOfBorrowStart, dateOfBorrowEnd, extensionCounter, reader.toLightDto(),
                state, returnDate, hardCopyExemplar.toLightDto(), fineDtos);
    }
}
