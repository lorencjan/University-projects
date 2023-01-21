package com.fit.vut.Library.dtos;

import com.fit.vut.Library.entities.*;
import com.fit.vut.Library.enums.ExemplarEnum;

import java.util.*;

public class ElectronicCopyExemplarDto extends ExemplarBaseDto {

    protected String filePath;

    private List<ElectronicCopyBorrowingDto> borrowings;

    public String getFilePath() {
        return filePath;
    }

    public void setFilePath(String filePath) {
        this.filePath = filePath;
    }

    public ElectronicCopyExemplarDto() {}

    public ElectronicCopyExemplarDto(Long id, ExemplarEnum state, boolean availability, int borrowPeriod, int maximumNumberOfExtension,
                                     TitleLightDto book, TitleLightDto magazine, List<ElectronicCopyBorrowingDto> borrowings, String filePath) {
        super(id, state, availability, borrowPeriod, maximumNumberOfExtension, book, magazine);
        this.borrowings = borrowings;
        this.filePath = filePath;
    }

    public List<ElectronicCopyBorrowingDto> getBorrowings() {
        return borrowings;
    }

    public void setBorrowings(List<ElectronicCopyBorrowingDto> borrowings) {
        this.borrowings = borrowings;
    }

    public ElectronicCopyExemplar toEntity() {
        Book bookEntity = getBook() == null ? null : getBook().toBookEntity();
        Magazine magazineEntity = getMagazine() == null ? null : getMagazine().toMagazineEntity();
        List<ElectronicCopyBorrowing> eBorrowings = getBorrowings() == null ? new ArrayList() : getBorrowings().stream().map(x -> x.toEntity()).toList();
        return new ElectronicCopyExemplar(id, state, availability, borrowPeriod, maximumNumberOfExtension,
                bookEntity, magazineEntity, eBorrowings, filePath);
    }
}
