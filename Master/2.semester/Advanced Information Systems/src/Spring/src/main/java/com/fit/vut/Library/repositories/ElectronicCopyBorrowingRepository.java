package com.fit.vut.Library.repositories;

import com.fit.vut.Library.entities.ElectronicCopyBorrowing;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

@Repository
public interface ElectronicCopyBorrowingRepository extends JpaRepository <ElectronicCopyBorrowing, Long> {}

