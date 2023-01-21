package com.fit.vut.Library.entities;

import com.fit.vut.Library.dtos.*;
import com.fit.vut.Library.enums.ExemplarEnum;

import java.util.*;
import javax.persistence.*;

@Entity
@Table(name = "electroniccopyexemplar")
public class ElectronicCopyExemplar extends Exemplar {

    @Column(name = "filePath ")
    protected String filePath ;

    @OneToMany(fetch = FetchType.LAZY, orphanRemoval = true, mappedBy = "electronicCopyExemplar")
    private List<ElectronicCopyBorrowing> electronicCopyBorrowings = new ArrayList<>();

    public String getFilePath() {
        return filePath;
    }

    public void setFilePath(String filePath) {
        this.filePath = filePath;
    }

    public ElectronicCopyExemplar() {}

    public ElectronicCopyExemplar(Long id) {
        this.id = id;
    }

    public ElectronicCopyExemplar(Long id, ExemplarEnum state, boolean availability, int borrowPeriod, int maximumNumberOfBorrow,
                                  Book book, Magazine magazine, List<ElectronicCopyBorrowing> borrowings, String filePath) {
        super(id, state, availability, borrowPeriod, maximumNumberOfBorrow, book, magazine);
        this.electronicCopyBorrowings = borrowings;
        this.filePath = filePath;
    }

    public ElectronicCopyExemplar(ExemplarEnum state, boolean availability, int borrowPeriod, int maximumNumberOfBorrow,
                                  Book book, Magazine magazine, List<ElectronicCopyBorrowing> borrowings, String filePath) {
        super(state, availability, borrowPeriod, maximumNumberOfBorrow, book, magazine);
        this.electronicCopyBorrowings = borrowings;
        this.filePath = filePath;
    }


    public List<ElectronicCopyBorrowing> getElectronicCopyBorrowings() {
        return electronicCopyBorrowings;
    }

    public void setElectronicCopyBorrowings(List<ElectronicCopyBorrowing> electronicCopyBorrowings) {
        this.electronicCopyBorrowings = electronicCopyBorrowings;
    }

    public void addElectronicCopyBorrowings(ElectronicCopyBorrowing electronicCopyBorrowing) {
        electronicCopyBorrowings.add(electronicCopyBorrowing);
        electronicCopyBorrowing.setElectronicCopyExemplar(this);
    }

    public void removeElectronicCopyBorrowings(ElectronicCopyBorrowing electronicCopyBorrowing) {
        electronicCopyBorrowings.remove(electronicCopyBorrowing);
        electronicCopyBorrowing.setElectronicCopyExemplar(this);
    }

    public ElectronicCopyExemplarDto toDto() {
        List<ElectronicCopyBorrowingDto> borrowingEntities = getElectronicCopyBorrowings() == null ? new ArrayList() : getElectronicCopyBorrowings().stream().map(x -> x.toDto()).toList();
        TitleLightDto bookDto = book == null ? null : book.toLightDto();
        TitleLightDto magazineDto = magazine == null ? null : magazine.toLightDto();
        return new ElectronicCopyExemplarDto(id, state, availability, borrowPeriod, maximumNumberOfExtension, bookDto, magazineDto, borrowingEntities, filePath);
    }
}
