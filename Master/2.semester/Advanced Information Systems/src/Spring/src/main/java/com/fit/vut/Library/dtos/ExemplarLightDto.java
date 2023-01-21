package com.fit.vut.Library.dtos;

import com.fit.vut.Library.entities.*;
import com.fit.vut.Library.enums.ExemplarEnum;

public class ExemplarLightDto extends ExemplarBaseDto {

    public ExemplarLightDto() {}

    public ExemplarLightDto(Long id, ExemplarEnum state, boolean availability, int borrowPeriod, int maximumNumberOfExtension, TitleLightDto book, TitleLightDto magazine) {
        super(id, state, availability, borrowPeriod, maximumNumberOfExtension, book, magazine);
    }

    public HardCopyExemplar toHardCopyExemplarEntity() { return new HardCopyExemplar(id); }

    public ElectronicCopyExemplar toElectronicCopyExemplarEntity() { return new ElectronicCopyExemplar(id); }
}
