package com.fit.vut.Library.entities;

import com.fit.vut.Library.dtos.ElectronicCopyBorrowingDto;

import java.util.Date;
import javax.persistence.*;

@Entity
@Table(name = "electroniccopyborrowing")
public class ElectronicCopyBorrowing extends Borrowing{

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name="electronicCopyExemplarId", nullable = false)
    private ElectronicCopyExemplar electronicCopyExemplar;

    public ElectronicCopyBorrowing() {}

    public ElectronicCopyBorrowing(Long id) {
        this.id = id;
    }

    public ElectronicCopyBorrowing(Long id, Date dateOfBorrowStart, Date dateOfBorrowEnd, int extensionCounter,
                                   Reader reader, ElectronicCopyExemplar exemplar) {
        super(id, dateOfBorrowStart, dateOfBorrowEnd, extensionCounter, reader);
        this.electronicCopyExemplar = exemplar;
    }

    public ElectronicCopyBorrowing(Date dateOfBorrowStart, Date dateOfBorrowEnd, int extensionCounter,
                                   Reader reader, ElectronicCopyExemplar exemplar) {
        super(dateOfBorrowStart, dateOfBorrowEnd, extensionCounter, reader);
        this.electronicCopyExemplar = exemplar;
    }


    public ElectronicCopyExemplar getElectronicCopyExemplar() {
        return electronicCopyExemplar;
    }

    public void setElectronicCopyExemplar(ElectronicCopyExemplar electronicCopyExemplar) {
        this.electronicCopyExemplar = electronicCopyExemplar;
    }

    public ElectronicCopyBorrowingDto toDto() {
        return new ElectronicCopyBorrowingDto(id, dateOfBorrowStart, dateOfBorrowEnd, extensionCounter, reader.toLightDto(), electronicCopyExemplar.toLightDto());
    }
}
