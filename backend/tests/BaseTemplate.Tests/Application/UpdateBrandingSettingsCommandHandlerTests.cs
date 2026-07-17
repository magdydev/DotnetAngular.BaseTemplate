using AutoMapper;
using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Settings.Commands.UpdateBrandingSettings;
using BaseTemplate.Application.Settings.Dtos;
using BaseTemplate.Domain.Common;
using BaseTemplate.Domain.Entities;
using BaseTemplate.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace BaseTemplate.Tests.Application;

public class UpdateBrandingSettingsCommandHandlerTests
{
    private readonly Mock<IBrandingSettingsRepository> _brandingSettingsRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IDomainEventDispatcher> _domainEventDispatcher = new();
    private readonly IMapper _mapper;
    private readonly UpdateBrandingSettingsCommandHandler _handler;

    public UpdateBrandingSettingsCommandHandlerTests()
    {
        _unitOfWork.Setup(u => u.BrandingSettings).Returns(_brandingSettingsRepository.Object);
        _unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var mapperConfig = new MapperConfiguration(cfg => cfg.CreateMap<BrandingSettings, BrandingSettingsDto>());
        _mapper = mapperConfig.CreateMapper();

        _handler = new UpdateBrandingSettingsCommandHandler(
            _unitOfWork.Object,
            _domainEventDispatcher.Object,
            _mapper,
            NullLogger<UpdateBrandingSettingsCommandHandler>.Instance);
    }

    [Fact]
    public async Task Handle_WhenNoSettingsExist_CreatesNewRow()
    {
        _brandingSettingsRepository.Setup(r => r.GetCurrentAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((BrandingSettings?)null);

        var command = new UpdateBrandingSettingsCommand("Acme", "assets/logo.svg", "#4F46E5", "#F59E0B");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.AppName.Should().Be("Acme");
        _brandingSettingsRepository.Verify(r => r.AddAsync(It.IsAny<BrandingSettings>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _domainEventDispatcher.Verify(
            d => d.DispatchAndClearEvents(It.IsAny<IEnumerable<BaseEntity>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenSettingsExist_UpdatesExistingRow()
    {
        var existing = BrandingSettings.CreateDefault("BaseTemplate", "assets/logo.svg", "#4F46E5", "#F59E0B");
        existing.ClearDomainEvents();
        _brandingSettingsRepository.Setup(r => r.GetCurrentAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var command = new UpdateBrandingSettingsCommand("Acme", "assets/new-logo.svg", "#000000", "#FFFFFF");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.AppName.Should().Be("Acme");
        result.PrimaryColor.Should().Be("#000000");
        _brandingSettingsRepository.Verify(r => r.AddAsync(It.IsAny<BrandingSettings>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
