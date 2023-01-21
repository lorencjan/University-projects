package com.fit.vut.Library.dtos;

import com.fit.vut.Library.entities.ElectronicCopyBorrowing;
import java.util.Date;

public class ElectronicCopyBorrowingDto extends BorrowingDto {

    private ExemplarLightDto exemplar;

    public ElectronicCopyBorrowingDto() {}

    public ElectronicCopyBorrowingDto(Long id) {
        this.id = id;
    }

    public ElectronicCopyBorrowingDto(Long id, Date dateOfBorrowStart, Date dateOfBorrowEnd, int borrowCounter,
                                      ReaderLightDto reader, ExemplarLightDto exemplar){
        super(id, dateOfBorrowStart, dateOfBorrowEnd, borrowCounter, reader);
        this.exemplar = exemplar;
    }


    public ExemplarLightDto getElectronicCopy() {
        return exemplar;
    }

    public void setElectronicCopy(ExemplarLightDto copy) {
        this.exemplar = copy;
    }

    public ElectronicCopyBorrowing toEntity() {
        return new ElectronicCopyBorrowing(id, dateOfBorrowStart, dateOfBorrowEnd, borrowCounter,
                reader.toEntity(), exemplar.toElectronicCopyExemplarEntity());
    }
}
