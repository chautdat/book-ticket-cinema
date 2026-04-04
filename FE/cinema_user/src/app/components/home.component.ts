import { Component, OnInit } from '@angular/core';

@Component({
  standalone: false,
  template: `
    <div class="home-page">
      <!-- Carousel Start -->
      <section class="home-hero">
        <div class="hero-frame">
          <div id="header-carousel" class="carousel slide" data-bs-ride="carousel">
            <div class="carousel-inner">
              <div class="carousel-item active">
                <img
                  class="hero-image"
                  src="assets/img/carousel/screen1.jpg"
                  alt="Banner DQP Cinema"
                />
              </div>
            </div>
          </div>
        </div>
      </section>
      <!-- Carousel End -->

      <section class="container-xxl home-intro-wrap">
        <div class="container">
          <div class="home-intro-card">
            <div class="intro-content">
              <span class="intro-eyebrow">DQP CINEMA</span>
              <h1>Trải nghiệm điện ảnh đỉnh cao</h1>
              <p>
                Đặt vé nhanh, chọn ghế dễ, theo dõi lịch chiếu và thưởng thức
                bom tấn mỗi ngày tại rạp gần bạn.
              </p>
            </div>
            <a class="intro-cta" [routerLink]="['/cinema']">
              <i class="fas fa-ticket-alt"></i>
              <span>{{ 'buy_tickets' | translate }}</span>
            </a>
          </div>
        </div>
      </section>

      <!-- Service Start -->
      <section class="container-xxl py-5 home-service-overview">
        <div class="container">
          <div class="row g-4">
          <div class="col-lg-4 col-md-6 wow fadeInUp" data-wow-delay="0.1s">
            <div class="service-card">
              <div class="service-icon">
                <img src="assets/img/order.png" alt="Chất lượng dịch vụ" />
              </div>
              <div class="service-content">
                <h5 class="mb-3">
                  {{ 'service' | translate }}
                </h5>
                <p>{{ 'service_c' | translate }}</p>
              </div>
            </div>
          </div>
          <div class="col-lg-4 col-md-6 wow fadeInUp" data-wow-delay="0.3s">
            <div class="service-card">
              <div class="service-icon">
                <img src="assets/img/group.png" alt="Đội ngũ thành viên" />
              </div>

              <div class="service-content">
                <h5 class="">
                  {{ 'member' | translate }}
                </h5>
                <p class="service-list">
                  {{ 'member_c1' | translate }} <br />
                  {{ 'member_c2' | translate }} <br />
                  {{ 'member_c3' | translate }}
                </p>
              </div>
            </div>
          </div>
          <div class="col-lg-4 col-md-6 wow fadeInUp" data-wow-delay="0.5s">
            <div class="service-card">
              <div class="service-icon">
                <img src="assets/img/trade.png" alt="Danh hiệu" />
              </div>
              <div class="service-content">
                <h5 class="mb-3">{{ 'title' | translate }}</h5>

                <p>
                  {{ 'title_c1' | translate
                  }}<span style="color:#FF0000">T Cinema</span>
                  {{ 'title_c2' | translate }}
                </p>
              </div>
            </div>
          </div>
        </div>
        </div>
      </section>
      <section class="container-xxl py-5">
        <div class="container">
          <div class="row g-4">
            <h1 class="home-payment-title">
              {{ 'payment' | translate }}
              <span style="color:#FF0000">DQP Cinema</span>
            </h1>
          </div>
          <div class="home-payment-row">
            <div class="payment-pill">Thanh toán tiền mặt tại quầy</div>
            <div class="payment-pill">ZaloPay (Sandbox)</div>
          </div>
        </div>
      </section>
      <!-- Service End -->

    <!-- Fact Start -->
    <div class="container-fluid fact bg-dark my-5 py-5">
      <div class="container">
        <div class="row g-4">
          <div
            class="col-md-6 col-lg-3 text-center wow fadeIn"
            data-wow-delay="0.1s"
          >
            <i class="fa fa-check fa-2x text-white mb-3"></i>
            <h2 class="text-white mb-2" data-toggle="counter-up">15</h2>
            <p class="text-white mb-0">Năm Kinh Nghiệm</p>
          </div>
          <div
            class="col-md-6 col-lg-3 text-center wow fadeIn"
            data-wow-delay="0.3s"
          >
            <!-- <i class="fa fa-users-cog fa-2x text-white mb-3"></i> -->
            <img src="assets/img/badge.png" width="50" height="50" />
            <h2 class="text-white mb-2" data-toggle="counter-up">500+</h2>
            <p class="text-white mb-0">Danh Hiệu</p>
          </div>
          <div
            class="col-md-6 col-lg-3 text-center wow fadeIn"
            data-wow-delay="0.5s"
          >
            <img src="assets/img/star.png" width="50" height="50" />
            <h2 class="text-white mb-2" data-toggle="counter-up">5.0/5</h2>
            <p class="text-white mb-0">Đánh Giá Dịch Vụ</p>
          </div>
          <div
            class="col-md-6 col-lg-3 text-center wow fadeIn"
            data-wow-delay="0.7s"
          >
            <img src="assets/img/partners.png" width="50" height="50" />
            <h2 class="text-white mb-2" data-toggle="counter-up">1000</h2>
            <p class="text-white mb-0">Đối Tác Toàn Cầu</p>
          </div>
        </div>
      </div>
    </div>
    <!-- Fact End -->

    <!-- Team Start -->
    <div class="container-xxl py-5">
      <div class="container">
        <div class="text-center wow fadeInUp" data-wow-delay="0.1s">
          <h1 class="mb-5">Thành Viên Trong Nhóm</h1>
        </div>
        <div class="row g-4 justify-content-center">
          <!-- Thêm justify-content-center -->

          <!-- Mỗi cột chuyển thành col-lg-4 để chia đều 3 cột ngang màn hình -->
          <div class="col-lg-4 col-md-6 wow fadeInUp" data-wow-delay="0.1s">
            <div class="team-item">
              <div class="position-relative overflow-hidden">
                <img class="img-fluid" src="assets/img/team-1.jpg" alt="" />
              </div>
              <div class="bg-light text-center p-4">
                <h5 class="fw-bold mb-0">Châu Tấn Đạt</h5>
              </div>
            </div>
          </div>

          <div class="col-lg-4 col-md-6 wow fadeInUp" data-wow-delay="0.3s">
            <div class="team-item">
              <div class="position-relative overflow-hidden">
                <img class="img-fluid" src="assets/img/team-2.jpg" alt="" />
              </div>
              <div class="bg-light text-center p-4">
                <h5 class="fw-bold mb-0">Nguyễn Phú Minh Quân</h5>
              </div>
            </div>
          </div>

          <div class="col-lg-4 col-md-6 wow fadeInUp" data-wow-delay="0.5s">
            <div class="team-item">
              <div class="position-relative overflow-hidden">
                <img class="img-fluid" src="assets/img/team-3.jpg" alt="" />
              </div>
              <div class="bg-light text-center p-4">
                <h5 class="fw-bold mb-0">Nguyễn Võ Tấn Phương</h5>
              </div>
            </div>
          </div>

          <div class="col-lg-4 col-md-6 wow fadeInUp" data-wow-delay="0.5s">
            <div class="team-item">
              <div class="position-relative overflow-hidden">
                <img class="img-fluid" src="assets/img/team-3.jpg" alt="" />
              </div>
              <div class="bg-light text-center p-4">
                <h5 class="fw-bold mb-0">Lê Gia Bảo</h5>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

      <!-- Team End -->
    </div>
  `,
  styles: [
    `
      .home-page {
        background: #f3f5fa;
      }

      .home-hero {
        margin-bottom: 0;
        padding-top: 16px;
      }

      .hero-frame {
        max-width: 1360px;
        margin: 0 auto;
        padding: 0 12px;
      }

      #header-carousel {
        border-radius: 18px;
        overflow: hidden;
        box-shadow: 0 12px 30px rgba(15, 23, 42, 0.16);
      }

      .hero-image {
        width: 100%;
        height: clamp(240px, 37vw, 460px);
        object-fit: cover;
        object-position: center;
        display: block;
      }

      .home-intro-wrap {
        margin-top: 16px;
      }

      .home-intro-card {
        display: flex;
        align-items: center;
        justify-content: space-between;
        gap: 18px;
        background: #fff;
        border: 1px solid #e7ecf3;
        border-radius: 16px;
        padding: 22px 24px;
        box-shadow: 0 8px 22px rgba(15, 23, 42, 0.08);
      }

      .intro-eyebrow {
        display: inline-block;
        font-size: 0.8rem;
        font-weight: 700;
        letter-spacing: 0.12em;
        color: #dc2626;
        margin-bottom: 6px;
      }

      .home-intro-card h1 {
        margin: 0;
        color: #0f172a;
        font-size: clamp(1.35rem, 2.6vw, 2rem);
        line-height: 1.22;
        font-weight: 750;
      }

      .home-intro-card p {
        margin: 10px 0 0;
        color: #475569;
        font-size: 1rem;
        max-width: 760px;
      }

      .intro-cta {
        display: inline-flex;
        align-items: center;
        justify-content: center;
        gap: 10px;
        min-width: 185px;
        height: 46px;
        padding: 0 20px;
        border-radius: 999px;
        background: linear-gradient(135deg, #ff4239, #e10600);
        color: #fff;
        text-decoration: none;
        font-weight: 700;
        box-shadow: 0 10px 22px rgba(225, 6, 0, 0.3);
        flex-shrink: 0;
        transition: transform 0.2s ease, box-shadow 0.2s ease;
      }

      .intro-cta:hover {
        transform: translateY(-1px);
        box-shadow: 0 14px 24px rgba(225, 6, 0, 0.35);
        color: #fff;
      }

      .home-service-overview {
        margin-top: 0;
        padding-top: 1.2rem !important;
      }

      .service-card {
        height: 100%;
        display: flex;
        align-items: flex-start;
        gap: 12px;
        background: #ffffff;
        border-radius: 14px;
        border: 1px solid #e8edf4;
        padding: 20px 18px;
        box-shadow: 0 8px 18px rgba(15, 23, 42, 0.06);
      }

      .service-icon {
        width: 58px;
        height: 58px;
        flex-shrink: 0;
        border-radius: 12px;
        display: grid;
        place-items: center;
        background: #f8fafc;
        border: 1px solid #edf2f8;
      }

      .service-icon img {
        width: 32px;
        height: 32px;
        object-fit: contain;
      }

      .service-content {
        flex: 1;
      }

      .service-card h5 {
        margin: 2px 0 6px;
        font-size: 1.2rem;
        font-weight: 700;
        color: #111827;
      }

      .service-card p {
        margin: 0;
        color: #4b5563;
        line-height: 1.55;
      }

      .service-list {
        margin-top: 0 !important;
      }

      .home-payment-title {
        text-align: center;
        margin-bottom: 0;
        color: #111827;
      }

      .home-payment-row {
        text-align: center;
        display: flex;
        justify-content: center;
        gap: 18px;
        flex-wrap: wrap;
        margin-top: 10px;
      }

      .payment-pill {
        background: #fff;
        border: 1px solid #e4e9f1;
        border-radius: 999px;
        padding: 10px 18px;
        font-weight: 600;
        box-shadow: 0 6px 14px rgba(15, 23, 42, 0.09);
      }

      @media (max-width: 768px) {
        .hero-frame {
          padding: 0 8px;
        }

        #header-carousel {
          border-radius: 14px;
        }

        .hero-image {
          height: clamp(210px, 50vw, 290px);
        }

        .home-intro-wrap {
          margin-top: 12px;
        }

        .home-intro-card {
          flex-direction: column;
          align-items: flex-start;
          padding: 16px;
        }

        .intro-cta {
          width: 100%;
        }

        .service-card {
          padding: 16px 14px;
        }

        .service-icon {
          width: 50px;
          height: 50px;
          border-radius: 10px;
        }

        .service-icon img {
          width: 28px;
          height: 28px;
        }

        .service-card h5 {
          font-size: 1rem;
        }
      }
    `,
  ],
})
export class HomeComponent implements OnInit {
  ngOnInit(): void {}
}
