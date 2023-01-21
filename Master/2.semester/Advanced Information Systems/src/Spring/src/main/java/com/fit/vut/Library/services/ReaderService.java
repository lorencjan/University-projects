package com.fit.vut.Library.services;

import com.fit.vut.Library.application.PasswordEncryptor;
import com.fit.vut.Library.dtos.*;
import com.fit.vut.Library.entities.Reader;
import com.fit.vut.Library.enums.RoleEnum;
import com.fit.vut.Library.repositories.ReaderRepository;
import com.fit.vut.Library.services.filters.UserFilter;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.web.server.ResponseStatusException;

import javax.persistence.EntityManager;
import javax.persistence.PersistenceContext;
import javax.persistence.criteria.*;
import java.util.*;

@Service
@Transactional
public class ReaderService {

    @PersistenceContext
    private EntityManager em;
    private final ReaderRepository repo;

    public ReaderService(ReaderRepository repo) {
        this.repo = repo;
    }

    public void create (FullUserDto u) throws Exception {
        if ( ! repo.findByEmail(u.getEmail()).isEmpty()) {
            throw new Exception();
        }
        Reader reader = new Reader(0L, u.getEmail(), PasswordEncryptor.encrypt(u.getPassword()), u.getName(),
                u.getSurname(), u.getStreet(), u.getHouseNumber(), u.getCity(), u.getPostcode(), RoleEnum.READER,
                new ArrayList(), new ArrayList(), new ArrayList<>());
        repo.save(reader);
    }

    public void update (ReaderDto r){
        Reader reader = repo.getById(r.getId());
        reader.setName(r.getName());
        reader.setSurname(r.getSurname());
        reader.setEmail(r.getEmail());
        reader.setStreet(r.getStreet());
        reader.setHouseNumber(r.getHouseNumber());
        reader.setCity(r.getCity());
        reader.setPostcode(r.getPostcode());
        repo.save(reader);
    }

    public boolean updatePassword(NewPasswordDto metadata){
        Reader reader = repo.getById(metadata.getUserId());
        boolean match = PasswordEncryptor.verify(metadata.getOldPassword(),reader.getHashPassword());
        if (match){
            String newPassword = PasswordEncryptor.encrypt(metadata.getPassword());
            reader.setHashPassword(newPassword);
            repo.save(reader);
            return true;
        }
        return false;
    }

    public void changePassword(ChangePasswordDto changePasswordDto) {
        List<Reader> readers = repo.findByEmail(changePasswordDto.getUserEmail());
        Reader reader = readers.get(0);
        String newPassword = PasswordEncryptor.encrypt(changePasswordDto.getPassword());
        reader.setHashPassword(newPassword);
        repo.save(reader);
    }

    public void delete (Long id){
        repo.delete(new Reader(id));
    }

    public ReaderDto getById (Long id){
        Optional<Reader> reader = repo.findById(id);
        return reader.isEmpty() ? null : reader.get().toDto();
    }

    public ReaderDto getByEmail (String email){
        List<Reader> reader = repo.findByEmail(email);
        return reader.isEmpty() ? null : reader.get(0).toDto();
    }

    public ReaderDto getByCredentials (CredentialsDto credentials){
        List<Reader> readers = repo.findByEmail(credentials.getEmail());
        if (readers.isEmpty())
            return null;

        Reader reader = readers.stream().findFirst().get();
        return PasswordEncryptor.verify(credentials.getPassword(), reader.getHashPassword()) ? reader.toDto() : null;
    }
    
    public List<ReaderDto> get(UserFilter filter) {
        if (filter == null)
            repo.findAll().stream().map(x -> x.toDto()).toList();

        CriteriaBuilder cb = em.getCriteriaBuilder();
        CriteriaQuery<Reader> cq = cb.createQuery(Reader.class);
        Root<Reader> quest = cq.from(Reader.class);
        List<Predicate> predicates = new ArrayList();

        if (filter.getName() != null)
            predicates.add(cb.like(quest.get("name"), "%" + filter.getName() + "%"));

        if (filter.getSurname() != null)
            predicates.add(cb.like(quest.get("surname"), "%" + filter.getSurname() + "%"));

        if (filter.getEmail() != null)
            predicates.add(cb.like(quest.get("email"), "%" + filter.getEmail() + "%"));

        if (filter.getStreet() != null)
            predicates.add(cb.like(quest.get("street"), "%" + filter.getStreet() + "%"));

        if (filter.getHouseNumber() != null)
            predicates.add(cb.like(quest.get("houseNumber"), "%" + filter.getHouseNumber() + "%"));

        if (filter.getCity() != null)
            predicates.add(cb.like(quest.get("city"), "%" + filter.getCity() + "%"));

        if (filter.getPostcode() != null)
            predicates.add(cb.like(quest.get("postcode"), "%" + filter.getPostcode() + "%"));

        if (filter.getRole() != null)
            predicates.add(cb.like(quest.get("role"), "%" + filter.getRole() + "%"));

        cq.select(quest).where(predicates.toArray(new Predicate[] {}));
        List<Reader> readers = em.createQuery(cq).getResultList();
        return readers.stream().map(x -> x.toDto()).toList();
    }


}
