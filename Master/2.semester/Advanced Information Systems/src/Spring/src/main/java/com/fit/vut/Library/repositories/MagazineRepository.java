package com.fit.vut.Library.repositories;

import com.fit.vut.Library.entities.Magazine;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

@Repository
public interface MagazineRepository extends JpaRepository<Magazine, Long> {}
